using MyMVCProject.Api.Entities;

namespace MyMVCProject.Api.Dtos.Folders;

public record FolderResponse(
    Guid Id,
    string Name,
    string EncryptedKey,
    string KeyEncryptedByRoot,
    string RootKeySalt,
    Guid OwnerId,
    Guid? ParentId,
    DateTime CreatedAt)
{
    public static FolderResponse From(Folder folder)
    {
        return new FolderResponse(
            folder.Id, 
            folder.Name, 
            folder.EncryptedKey, 
            folder.KeyEncryptedByRoot, 
            folder.RootKeySalt,
            folder.OwnerId, 
            folder.ParentFolderId,
            folder.CreatedAt);
    }
};