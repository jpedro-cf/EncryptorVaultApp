using MyMVCProject.Api.Entities;

namespace MyMVCProject.Api.Dtos.Folders;

public record FolderResponse(
    Guid Id,
    string Name,
    EncryptionKey EncryptedKey,
    EncryptionKey KeyEncryptedByRoot,
    Guid OwnerId,
    Guid? ParentId,
    DateTime CreatedAt)
{
    public static FolderResponse From(Folder folder)
    {
        return new FolderResponse(
            folder.Id, 
            folder.Name, 
            folder.GetEncryptedKey(), 
            folder.GetRootKey(),
            folder.OwnerId, 
            folder.ParentFolderId,
            folder.CreatedAt);
    }
};