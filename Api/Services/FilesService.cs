using Microsoft.EntityFrameworkCore;
using MyMVCProject.Api.Dtos.Files;
using MyMVCProject.Api.Entities;
using MyMVCProject.Api.Global;
using MyMVCProject.Api.Infra.Storage;
using MyMVCProject.Config;
using File = MyMVCProject.Api.Entities.File;

namespace MyMVCProject.Api.Services;

public class FilesService(AppDbContext ctx, AmazonS3 amazonS3)
{
    public async Task<Result<InitiateUploadResponse>> Upload(Guid userId, UploadFileRequest data)
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
            var parentFolder = await ctx.Folders.FirstAsync(f => f.Id == data.ParentFolderId);
            file.ParentFolderId = parentFolder.Id;
        }

        ctx.Files.Add(file);
        await ctx.SaveChangesAsync();

        var uploadInitiated = await amazonS3.InitiateUpload(file);

        return Result<InitiateUploadResponse>.Success(
            new InitiateUploadResponse(uploadInitiated.UploadId,uploadInitiated.Key,uploadInitiated.Urls));
    }

    public async Task<Result<UploadCompletedResponse>> CompleteUpload(CompleteUploadRequest data)
    {
        return Result<UploadCompletedResponse>.Success(await amazonS3.CompletedUpload(data));
    }

    public async Task ConfirmUpload(string fileId, long fileSize)
    {
        var file = await ctx.Files.FirstAsync(f => f.Id == Guid.Parse(fileId));

        file.Status = FileStatus.Completed;
        file.Size = fileSize;

        await ctx.SaveChangesAsync();
    }
}