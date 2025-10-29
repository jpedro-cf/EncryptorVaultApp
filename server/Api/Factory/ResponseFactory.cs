using EncryptionApp.Api.Dtos.Files;
using EncryptionApp.Api.Dtos.Items;
using EncryptionApp.Api.Infra.Storage;
using File = EncryptionApp.Api.Entities.File;

namespace EncryptionApp.Api.Factory;

public class ResponseFactory(AmazonS3 amazonS3)
{
    public async Task<FileResponse> CreateFileResponse(File file, bool withRootKey)
    {
        var url = await amazonS3.GeneratePresignedUrl(file.StorageKey);

        return new FileResponse(
            file.Id,
            EncryptedData.From(file.Name), 
            file.StorageKey,
            file.Size,
            file.ContentType,
            url,
            EncryptedData.From(file.EncryptedKey),
            withRootKey ? EncryptedData.From(file.KeyEncryptedByRoot) : null,
            file.ParentFolderId,
            file.CreatedAt);
    }
}