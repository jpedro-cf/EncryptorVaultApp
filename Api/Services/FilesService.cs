using Microsoft.EntityFrameworkCore;
using MyMVCProject.Api.Dtos.Files;
using MyMVCProject.Api.Global;
using MyMVCProject.Api.Infra.Security;
using MyMVCProject.Api.Infra.Storage;
using MyMVCProject.Config;
using File = MyMVCProject.Api.Entities.File;

namespace MyMVCProject.Api.Services;

public class FilesService(AppDbContext ctx, AmazonS3 amazonS3)
{
    public async Task<UploadResponse> Upload(Guid userId, UploadFileRequest data)
    {
        var parentFolder = await ctx.Folders.FirstAsync(f => f.Id == data.ParentFolderId);
        
        // Verify if keys provided are correct by trying to decrypt with them.
        // If is not correct, it will throw an error internally
        var parentEncryptionKey = EncryptionHandler.Decrypt(parentFolder.EncryptedKey, data.EncryptionKey);
        EncryptionHandler.Decrypt(parentFolder.KeyEncryptedByRoot, data.RootEncryptionKey);

        var encryptionKey = EncryptionHandler.GenerateRandomAes256Key();

        var file = new File
        {
            Name = data.FileName,
            Size = data.FileSize,
            OwnerId = userId,
            StorageKey = $"{userId}/{Guid.NewGuid()}",
            EncryptedKey = EncryptionHandler.Encrypt(encryptionKey, parentEncryptionKey),
            KeyEncryptedByRoot = EncryptionHandler.Encrypt(encryptionKey, data.RootEncryptionKey),
            RootKeySalt = parentFolder.RootKeySalt,
            ParentFolderId = parentFolder.Id
        };

        ctx.Files.Add(file);
        await ctx.SaveChangesAsync();

        var uploadInitiated = await amazonS3.InitiateUpload(file, encryptionKey);

        return new UploadResponse(
            uploadInitiated.UploadId,
            uploadInitiated.Key,
            uploadInitiated.Urls,
            encryptionKey,
            encryptionKey.ToMd5Base64());
    }

    public async Task<UploadCompletedResponse> CompleteUpload(CompleteUploadRequest data)
    {
        return await amazonS3.CompletedUpload(data);
    }
}