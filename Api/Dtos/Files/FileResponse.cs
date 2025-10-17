using File = MyMVCProject.Api.Entities.File;

namespace MyMVCProject.Api.Dtos.Files;

public record FileResponse(
    Guid Id,
    EncryptedData EncryptedFileName,
    string StorageKey,
    long Size,
    string ContentType,
    EncryptedData EncryptedKey,
    EncryptedData? KeyEncryptedByRoot,
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

    public static FileResponse WithoutRootKey(File file)
    {
        return new FileResponse(
            file.Id, 
            EncryptedData.From(file.Name), 
            file.StorageKey,
            file.Size,
            file.ContentType,
            EncryptedData.From(file.EncryptedKey), 
            null, 
            file.ParentFolderId,
            file.CreatedAt);
    }
}