using File = MyMVCProject.Api.Entities.File;

namespace MyMVCProject.Api.Dtos.Files;

public record FileResponse(
    Guid Id,
    string Name,
    string StorageKey,
    string EncryptedKey,
    string KeyEncryptedByRoot,
    string RootKeySalt,
    Guid OwnerId,
    Guid ParentId,
    DateTime CreatedAt)
{
    public static FileResponse From(File file)
    {
        return new FileResponse(
            file.Id, 
            file.Name, 
            file.StorageKey,
            file.EncryptedKey, 
            file.KeyEncryptedByRoot, 
            file.RootKeySalt,
            file.OwnerId, 
            file.ParentFolderId,
            file.CreatedAt);
    }
}