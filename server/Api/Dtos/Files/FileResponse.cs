using EncryptionApp.Api.Dtos.Items;
using File = EncryptionApp.Api.Entities.File;

namespace EncryptionApp.Api.Dtos.Files;

public record FileResponse(
    Guid Id,
    EncryptedData EncryptedName,
    string StorageKey,
    long Size,
    string ContentType,
    EncryptedData EncryptedKey,
    EncryptedData KeyEncryptedByRoot,
    Guid? ParentId,
    DateTime CreatedAt)
{
    public static FileResponse From(File file)
    {
        return new FileResponse(
            file.Id, 
            EncryptedData.From(file.Name), 
            file.StorageKey,
            file.Size,
            file.ContentType,
            EncryptedData.From(file.EncryptedKey), 
            EncryptedData.From(file.KeyEncryptedByRoot),
            file.ParentFolderId,
            file.CreatedAt);
    }
}