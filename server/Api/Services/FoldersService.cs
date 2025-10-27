using EncryptionApp.Api.Dtos.Folders;
using EncryptionApp.Api.Entities;
using EncryptionApp.Api.Factory;
using EncryptionApp.Api.Global;
using EncryptionApp.Api.Global.Errors;
using EncryptionApp.Api.Global.Helpers;
using EncryptionApp.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EncryptionApp.Api.Services;

public class FoldersService(AppDbContext ctx, ItemResponseFactory itemResponseFactory)
{
    public async Task<Result<FolderResponse>> Create(Guid userId, CreateFolderRequest data)
    {
        if (data.ParentId is null)
        {
            var folder = Folder.CreateRoot(data.Name, userId, data.EncryptedKey, data.KeyEncryptedByRoot);

            ctx.Folders.Add(folder);
            await ctx.SaveChangesAsync();

            return Result<FolderResponse>.Success(
                await itemResponseFactory.CreateFolderResponse(folder, true));
        }

        var parent = await ctx.Folders.FirstOrDefaultAsync(f => f.Id == data.ParentId);
        if (parent == null)
        {
            return Result<FolderResponse>.Failure(
                new NotFoundError("Parent folder not found."));
        }        

        var subFolder = Folder.CreateSubFolder(
            data.Name,
            userId,
            data.ParentId.Value,
            data.EncryptedKey,
            data.KeyEncryptedByRoot);

        ctx.Folders.Add(subFolder);
        await ctx.SaveChangesAsync();

        return Result<FolderResponse>.Success(
            await itemResponseFactory.CreateFolderResponse(subFolder, true));
    }

    public async Task<Result<FolderResponse>> GetFolder(Guid folderId, Guid? userId, GetFolderRequest data)
    {
        var folder = await ctx.Folders.FirstOrDefaultAsync(f => f.Id == folderId);
        if (folder == null)
        {
            return Result<FolderResponse>.Failure(new NotFoundError("Folder not found."));
        }
        
        if (!data.ShareId.IsNullOrEmpty())
        {
            var sharedItem = await ctx.SharedItems.FirstOrDefaultAsync(s => 
                s. Id == Guid.Parse(data.ShareId!));
            
            if (sharedItem == null)
            {
                return Result<FolderResponse>.Failure(new NotFoundError("Shared item not found."));
            }
            if (sharedItem.OwnerId != folder.OwnerId)
            {
                return Result<FolderResponse>.Failure(
                    new ForbiddenError("You're not allowed to view this folder."));
            }
            
            return Result<FolderResponse>.Success(
                await itemResponseFactory.CreateFolderResponse(folder, false));
        }
        
        if (userId == null)
        {
            return Result<FolderResponse>.Failure(
                new ForbiddenError("You're not allowed to view this folder"));
        }

        return Result<FolderResponse>.Success(
            await itemResponseFactory.CreateFolderResponse(folder, true));
    }

    public async Task<Result<bool>> DeleteFolder(Guid userId, Guid folderId)
    {
        var folder = await ctx.Folders.FirstOrDefaultAsync(f => f.Id == folderId && f.OwnerId == userId);
        if (folder == null)
        {
            return Result<bool>.Failure(new NotFoundError("Folder not found."));
        }

        var transaction = await ctx.Database.BeginTransactionAsync();
        try
        {
            // get all files recursively
            var filesToDelete = await ctx.Files
                .FromSqlInterpolated($@"
                    WITH RECURSIVE RecursiveFolders AS (
                        SELECT Id FROM Folders WHERE Id = {folderId}
                        UNION ALL
                        SELECT f.Id FROM Folders f
                        INNER JOIN RecursiveFolders rf ON f.ParentFolderId = rf.Id
                    )
                    SELECT f.* FROM Files f
                    INNER JOIN RecursiveFolders rf ON f.ParentFolderId = rf.Id
                ")
                .ToListAsync();

            var fileSizeGroupedByContentType = filesToDelete
                .GroupBy(f => f.ContentType.ToContentTypeEnum())
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(f => f.Size)
                );

            // update storage usage
            foreach (var group in fileSizeGroupedByContentType)
            {
                var storageUsage = await ctx.StorageUsage.FirstAsync(s => s.ContentType == group.Key);
                storageUsage.TotalSize -= group.Value;
            }

            ctx.Folders.Remove(folder);
            await transaction.CommitAsync();
            return Result<bool>.Success(true);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return Result<bool>.Failure(new InternalServerError("An error occured while deleting this folder."));
        }
    }
}