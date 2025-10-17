using MyMVCProject.Api.Entities;

namespace MyMVCProject.Api.Dtos.Folders;

public record FolderResponse(
    Guid Id,
    EncryptedData EncryptedFolderName,
    EncryptedData EncryptedKey,
    EncryptedData? KeyEncryptedByRoot,
    Guid? ParentId,
    DateTime CreatedAt)
{
    public static FolderResponse From(Folder folder)
    {
        return new FolderResponse(
            folder.Id, 
            EncryptedData.From(folder.Name), 
            EncryptedData.From(folder.EncryptedKey), 
            EncryptedData.From(folder.KeyEncryptedByRoot),
            folder.ParentFolderId,
            folder.CreatedAt);
    }
    
    public static FolderResponse WithoutRootKey(Folder folder)
    {
        return new FolderResponse(
            folder.Id, 
            EncryptedData.From(folder.Name), 
            EncryptedData.From(folder.EncryptedKey), 
            null,
            folder.ParentFolderId,
            folder.CreatedAt);
    }
};