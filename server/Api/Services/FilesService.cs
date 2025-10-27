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
using QRCoder.Extensions;
using File = EncryptionApp.Api.Entities.File;

namespace EncryptionApp.Api.Services;

public class FilesService(
    AppDbContext ctx, 
    StorageUsageService storageUsageService, 
    AmazonS3 amazonS3,
    ItemResponseFactory itemResponseFactory)
{
    public async Task<Result<InitiateUploadResponse>> Upload(Guid userId, UploadFileRequest data)
    {
        if (await storageUsageService.StorageLimitExceeded(userId, data.FileSize))
        {
            return Result<InitiateUploadResponse>.Failure(
                new StorageLimitExceededError("You've reached your storage limit."));
        }
        
        try
        {
            var file = new File
            {
                Name = data.FileName,
                Status = FileStatus.Pending,
                StorageKey = $"{userId}/{Guid.NewGuid()}",
                OwnerId = userId,
                ContentType = data.ContentType,
                Size = data.FileSize,
                EncryptedKey = data.EncryptedKey,
                KeyEncryptedByRoot = data.KeyEncryptedByRoot,
            };
        
            if (data.ParentFolderId != null)
            {
                var parentFolder = await ctx.Folders.FirstOrDefaultAsync(f => f.Id == data.ParentFolderId);
                if (parentFolder == null)
                {
                    return Result<InitiateUploadResponse>.Failure(
                        new NotFoundError("Parent folder not found."));
                }
                file.ParentFolderId = parentFolder.Id;
            }

            var uploadInitiated = await amazonS3.InitiateMultiPartUpload(file);
            file.UploadId = uploadInitiated.UploadId;
            
            ctx.Files.Add(file);
            await ctx.SaveChangesAsync();      

            return Result<InitiateUploadResponse>.Success(uploadInitiated);
        }
        catch (Exception e)
        {
            return Result<InitiateUploadResponse>.Failure(
                new UnprocessableEntityError("An error occured while initiating the upload."));
        }
    }

    public async Task<Result<ItemResponse>> CompleteUpload(Guid userId, CompleteUploadRequest data)
    {
        var file = await ctx.Files.FirstOrDefaultAsync(f => 
            f.OwnerId == userId && f.Id == Guid.Parse(data.FileId) && f.UploadId == data.UploadId);

        if (file == null)
        {
            return Result<ItemResponse>.Failure(
                new NotFoundError("File not found. Check if you provided the correct data."));
        }
        
        var uploadedParts = await amazonS3.ListUploadParts(data.Key, data.UploadId);

        var notUploaded = uploadedParts.Any(p => p.Size == null);
        var totalSize = uploadedParts.Sum(p => p.Size ?? 0L);
        var storageExceeded = await storageUsageService.StorageLimitExceeded(userId, totalSize);

        if (notUploaded || storageExceeded)
        {
            await CancelUpload(userId, Guid.Parse(data.FileId), 
                new CancelUploadRequest(data.Key, data.UploadId));
        }
        
        if (notUploaded)
        {
            return Result<ItemResponse>.Failure(
                new UnprocessableEntityError("You didn't upload one of the parts."));
        }
        if (storageExceeded)
        {
            return Result<ItemResponse>.Failure(
                new StorageLimitExceededError("You've reached your storage limit."));   
        }
        
        await using var transaction = await ctx.Database.BeginTransactionAsync();
        try
        {
            // update actual file size in case the user manipulated the request to look like he uploaded a smaller file
            file.Status = FileStatus.Completed;
            file.Size = totalSize;
            var contentType = file.ContentType.ToContentTypeEnum();
            
            // Pessimistic Lock
            var storageUsage = await ctx.StorageUsage
                .FromSqlInterpolated($@"
                    SELECT * FROM ""StorageUsage""
                    WHERE ""UserId"" = {userId} AND ""ContentType"" = {contentType.ToString()}
                    FOR UPDATE")
                .SingleAsync();
            
            // update storage usage
            storageUsage.TotalSize += totalSize;
                
            await ctx.SaveChangesAsync();
            await amazonS3.CompleteMultiPartUpload(data);
            await transaction.CommitAsync();
            
            return Result<ItemResponse>.Success(
                await itemResponseFactory.CreateFrom(file, true));
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return Result<ItemResponse>.Failure(
                new UnprocessableEntityError("An error occurred while completing the upload."));
        }
    }

    private async Task<Result<bool>> CancelUpload(Guid userId, Guid fileId, CancelUploadRequest data)
    {
        await using var transaction = await ctx.Database.BeginTransactionAsync();
        try
        {
            var file = await ctx.Files.FirstOrDefaultAsync(f =>
                f.Id == fileId && f.OwnerId == userId && f.UploadId == data.UploadId);

            if (file == null)
            {
                return Result<bool>.Failure(
                    new NotFoundError("File not found. Check if you provided the correct data."));
            }

            ctx.Files.Remove(file);
            await ctx.SaveChangesAsync();
            await amazonS3.AbortMultiPartUpload(data.Key, data.UploadId);
            await transaction.CommitAsync();
            
            return Result<bool>.Success(true);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return Result<bool>.Failure(
                new InternalServerError("An error occured while cancelling upload."));
        }
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
                    new NotFoundError("File not found or upload was not completed correctly."));
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