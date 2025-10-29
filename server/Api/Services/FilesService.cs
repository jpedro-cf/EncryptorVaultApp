using EncryptionApp.Api.Dtos.Files;
using EncryptionApp.Api.Dtos.Items;
using EncryptionApp.Api.Entities;
using EncryptionApp.Api.Factory;
using EncryptionApp.Api.Global;
using EncryptionApp.Api.Global.Errors;
using EncryptionApp.Api.Infra.Storage;
using EncryptionApp.Config;
using Microsoft.EntityFrameworkCore;
using EncryptionApp.Api.Global.Helpers;
using Microsoft.IdentityModel.Tokens;
using File = EncryptionApp.Api.Entities.File;

namespace EncryptionApp.Api.Services;

public class FilesService(
    AppDbContext ctx, 
    StorageUsageService storageUsageService, 
    AmazonS3 amazonS3,
    ResponseFactory responseFactory)
{
    public async Task<Result<FileResponse>> GetFile(Guid fileId, Guid? userId, GetFileRequest data)
    {
        // TODO: Use joins instead of querying each one
        var file = await ctx.Files.FirstOrDefaultAsync(f => f.Id == fileId);
        if (file == null)
        {
            return Result<FileResponse>.Failure(new NotFoundError("File not found."));
        }
        
        if (!data.ShareId.IsNullOrEmpty())
        {
            var sharedItem = await ctx.SharedItems.FirstOrDefaultAsync(s => 
                s. Id == Guid.Parse(data.ShareId!));
            
            if (sharedItem == null || sharedItem.OwnerId != file.OwnerId)
            {
                return Result<FileResponse>.Failure(
                    new ForbiddenError("You're not allowed to view this file."));
            }

            return Result<FileResponse>.Success(
                await responseFactory.CreateFileResponse(file,false));
        }
        
        if (userId == null)
        {
            return Result<FileResponse>.Failure(
                new ForbiddenError("You're not allowed to view this file"));
        }

        return Result<FileResponse>.Success(
            await responseFactory.CreateFileResponse(file,true));
    }
    
    public async Task<Result<bool>> DeleteFile(Guid userId, Guid fileId)
    {
        await using var transaction = await ctx.Database.BeginTransactionAsync();
        try
        {
            var file = await ctx.Files.FirstOrDefaultAsync(f => 
                f.Id == fileId && f.OwnerId == userId && f.Status == FileStatus.Completed);

            if (file == null)
            {
                return Result<bool>.Failure(
                    new NotFoundError("File not found or upload was not completed."));
            }

            var contentType = file.ContentType.ToContentTypeEnum();
            var storageUsage = await ctx.StorageUsage.FirstAsync(s => 
                s.UserId == userId && s.ContentType == contentType);

            storageUsage.TotalSize -= file.Size;
            
            ctx.Files.Remove(file);
            await ctx.SaveChangesAsync();

            await amazonS3.DeleteObject(file.StorageKey);
            
            await transaction.CommitAsync();
            return Result<bool>.Success(true);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return Result<bool>.Failure(
                new InternalServerError("An error occured while deleting the file."));
        }
    }
}