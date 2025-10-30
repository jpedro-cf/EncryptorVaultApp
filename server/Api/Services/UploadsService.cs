using EncryptionApp.Api.Dtos.Files;
using EncryptionApp.Api.Dtos.Items;
using EncryptionApp.Api.Entities;
using EncryptionApp.Api.Global;
using EncryptionApp.Api.Global.Errors;
using EncryptionApp.Api.Global.Helpers;
using EncryptionApp.Api.Infra.Storage;
using EncryptionApp.Config;
using Microsoft.EntityFrameworkCore;
using File = EncryptionApp.Api.Entities.File;

namespace EncryptionApp.Api.Services;

public class UploadsService(
    AppDbContext ctx, 
    StorageUsageService storageUsageService, 
    AmazonS3 amazonS3)
{
    public async Task<Result<InitiateUploadResponse>> Upload(Guid userId, UploadFileRequest data)
    {
        try
        {
            var file = new File
            {
                Name = data.FileName,
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
    
    public async Task<Result<ItemResponse>> CompleteUpload(Guid userId, Guid fileId, CompleteUploadRequest data)
    {
        await using var transaction = await ctx.Database.BeginTransactionAsync();
        try
        {
            var file = await ctx.Files.FirstOrDefaultAsync(f => 
                f.OwnerId == userId && f.Id == fileId && f.UploadId == data.UploadId);

            if (file == null)
            {
                return Result<ItemResponse>.Failure(
                    new NotFoundError("File not found. Check if you provided the correct data."));
            }
            
            // trust the source of truth, not the client
            var uploadedParts = await amazonS3.ListUploadParts(data.Key, data.UploadId);

            var notUploaded = uploadedParts.Any(p => p.Size == null);
            var totalSize = uploadedParts.Sum(p => p.Size ?? 0L);
        
            var storageExceeded = await storageUsageService.StorageLimitExceededWithLock(userId, totalSize);

            if (notUploaded || storageExceeded)
            {
                await CancelUpload(file, new CancelUploadRequest(data.Key, data.UploadId));
                AppError error = notUploaded
                    ? new UnprocessableEntityError("You didn't upload one of the parts.")
                    : new StorageLimitExceededError("You've reached your storage limit.");

                await transaction.CommitAsync();
                return Result<ItemResponse>.Failure(error);
            }
            
            // update actual file size in case the user manipulated the request to look like he uploaded a smaller file
            file.Status = FileStatus.Completed;
            file.Size = totalSize;
            var contentType = file.ContentType.ToContentTypeEnum();
            
            var storageUsage = await ctx.StorageUsage
                .Where(s => s.ContentType == contentType)
                .FirstAsync();
            
            storageUsage.TotalSize += totalSize;
                
            await ctx.SaveChangesAsync();
            await amazonS3.CompleteMultiPartUpload(data);
            await transaction.CommitAsync();
            
            return Result<ItemResponse>.Success(ItemResponse.From(file, true));
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return Result<ItemResponse>.Failure(
                new UnprocessableEntityError("An error occurred while completing the upload."));
        }
    }
    
    private async Task<Result<bool>> CancelUpload(File file, CancelUploadRequest data)
    {
        ctx.Files.Remove(file);
        await ctx.SaveChangesAsync();
        await amazonS3.AbortMultiPartUpload(data.Key, data.UploadId);
        
        return Result<bool>.Success(true);
    }
    
    public async Task<Result<bool>> CancelUploadWithTransaction(Guid userId, Guid fileId, CancelUploadRequest data)
    {
        await using var transaction = await ctx.Database.BeginTransactionAsync();
        try
        {
            var file = await ctx.Files.FirstOrDefaultAsync(f =>
                f.Id == fileId && f.OwnerId == userId);

            if (file == null)
            {
                return Result<bool>.Failure(new NotFoundError("File not found."));
            }
            
            var result = await CancelUpload(file, data);
            await transaction.CommitAsync();

            return result;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return Result<bool>.Failure(
                new InternalServerError("An error occured while cancelling upload."));
        }
    }
}