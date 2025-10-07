using File = MyMVCProject.Api.Entities.File;

namespace MyMVCProject.Api.Dtos.Files;

public record FileResponse(
    Guid Id,
    string Name,
    string StorageKey,
    EncryptionKey EncryptedKey,
    EncryptionKey KeyEncryptedByRoot,
    Guid OwnerId,
    Guid? ParentId,
    DateTime CreatedAt)
{
    public static FileResponse From(File file)
    {
        return new FileResponse(
            file.Id, 
            file.Name, 
            file.StorageKey,
            file.GetEncryptedKey(), 
            file.GetRootKey(), 
            file.OwnerId, 
            file.ParentFolderId,
            file.CreatedAt);
    }
}