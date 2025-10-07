using Microsoft.EntityFrameworkCore;
using MyMVCProject.Api.Dtos.Files;
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
            Size = data.FileSize,
            OwnerId = userId,
            StorageKey = $"{userId}/{Guid.NewGuid()}",
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
}