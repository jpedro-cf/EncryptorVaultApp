using EncryptionApp.Api.Entities;
using EncryptionApp.Api.Infra.Storage;
using EncryptionApp.Config;
using Microsoft.EntityFrameworkCore;

namespace EncryptionApp.Api.Workers;

public class DeletionBackgroundTask(
    BackgroundTaskQueue queue, 
    IServiceScopeFactory serviceScopeFactory,
    ILogger<DeletionBackgroundTask> logger,
    AmazonS3 amazonS3)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var task = await queue.Dequeue(stoppingToken);
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var handlers = new Dictionary<BackgroundTaskType, Func<AppDbContext, Guid, Task>>
                {
                    {BackgroundTaskType.Folder, HandleFolder},
                    {BackgroundTaskType.User, HandleUser},
                    {BackgroundTaskType.File, HandleFile},
                };

                await handlers[task.Type](dbContext, task.Id);
            }
        }
    }

    private async Task HandleUser(AppDbContext ctx, Guid userId)
    {
        await using var transaction = await ctx.Database.BeginTransactionAsync();
        try
        {
            logger.LogInformation($"Starting user '{userId}' deletion at {DateTime.UtcNow}");
            
            var content = await ctx.Users
                .Include(u => u.Files)
                .Where(u => u.Id == userId)
                .AsSplitQuery()
                .Select(x => new {User = x, Files = x.Files})
                .FirstOrDefaultAsync();

            if (content == null)
            {
                logger.LogInformation($"User '{userId} not found. Finishing task...'");
                return;
            }

            // cascade files, folders & shared links
            ctx.Users.Remove(content.User);
            await ctx.SaveChangesAsync();
            
            var files = content.Files;
            
            logger.LogInformation($"Found {files.Count} items for user '{userId}' at {DateTime.UtcNow}");
            
            var tasks = files
                .Select(item => 
                    item.Status == FileStatus.Pending 
                        ? amazonS3.AbortMultiPartUpload(item.StorageKey, item.UploadId) 
                        : amazonS3.DeleteObject(item.StorageKey))
                .ToList();
            
            await Task.WhenAll(tasks);

            await transaction.CommitAsync();
            logger.LogInformation($"Finished user '{userId}' deletion at {DateTime.UtcNow}");
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.LogError($"User deletion failed for user '{userId}' at {DateTime.UtcNow}");
        }
    }
    
    private async Task HandleFolder(AppDbContext ctx, Guid folderId)
    {
        await using var transaction = await ctx.Database.BeginTransactionAsync();
        try
        {
            logger.LogInformation($"Starting deletion of folder '{folderId}' at {DateTime.UtcNow}");
            var folder = await ctx.Folders.FirstOrDefaultAsync(f => f.Id == folderId);
            if (folder == null)
            {
                logger.LogInformation($"Folder '{folderId}' not found. Finishing task...");
                return;
            }
            
            var files = await ctx.Files
                .FromSqlInterpolated($@"
                    WITH RECURSIVE RecursiveFolders AS (
                        SELECT ""Id"" FROM ""Folders"" WHERE ""Id"" = {folderId}
                        UNION ALL
                        SELECT f.""Id"" FROM ""Folders"" f
                        INNER JOIN RecursiveFolders rf ON f.""ParentFolderId"" = rf.""Id""
                    )
                    SELECT f.* FROM ""Files"" f
                    JOIN RecursiveFolders rf ON f.""ParentFolderId"" = rf.""Id""
                ")
                .ToListAsync();
            
            logger.LogInformation($"Found '{files.Count}' sub files for folder '{folderId}' at {DateTime.UtcNow}");
            
            // cascade files
            ctx.Folders.Remove(folder);
            await ctx.SaveChangesAsync();
        
            var tasks = files
                .Select(item => 
                    item.Status == FileStatus.Pending 
                        ? amazonS3.AbortMultiPartUpload(item.StorageKey, item.UploadId) 
                        : amazonS3.DeleteObject(item.StorageKey))
                .ToList();
            
            await Task.WhenAll(tasks);
            await transaction.CommitAsync();
            
            logger.LogInformation($"Deletion of folder '{folderId}' completed at {DateTime.UtcNow}");
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.LogError($"Failed deletion of folder '{folderId}' at {DateTime.UtcNow}");
        }
    }
    
    private async Task HandleFile(AppDbContext ctx, Guid fileId)
    {
        await using var transaction = await ctx.Database.BeginTransactionAsync();
        try
        {
            logger.LogInformation($"Starting deletion of file '{fileId}' at {DateTime.UtcNow}");
            var file = await ctx.Files.FirstOrDefaultAsync(f => f.Id == fileId);
            if (file == null)
            {
                logger.LogInformation($"File '{fileId}' not found. Finishing task...");
                return;
            }
            
            ctx.Files.Remove(file);
            await ctx.SaveChangesAsync();

            if (file.Status == FileStatus.Pending)
            {
                await amazonS3.AbortMultiPartUpload(file.StorageKey, file.UploadId);
            }
            else
            {
                await amazonS3.DeleteObject(file.StorageKey);
            }
            
            await transaction.CommitAsync();
            
            logger.LogInformation($"Deletion of file '{fileId}' completed at {DateTime.UtcNow}");
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.LogError($"Failed deletion of file '{fileId}' at {DateTime.UtcNow}");
        }
    }
}