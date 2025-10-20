using EncryptionApp.Api.Dtos.Items;
using EncryptionApp.Api.Entities;

namespace EncryptionApp.Api.Dtos.Folders;

public record FolderResponse(
    Guid Id,
    EncryptedData EncryptedName,
    EncryptedData EncryptedKey,
    EncryptedData? KeyEncryptedByRoot,
    Guid? ParentId,
    List<ItemResponse> Children,
    DateTime CreatedAt)
{
    public static FolderResponse From(Folder folder, bool withRootKey)
    {
        var children = folder.Folders
            .Select(f => ItemResponse.From(f, withRootKey))
            .ToList();
        
        var subItems = folder.Files
            .Select(f => ItemResponse.From(f, withRootKey))
            .ToList();
        
        children.AddRange(subItems);
        
        return new FolderResponse(
            folder.Id, 
            EncryptedData.From(folder.Name), 
            EncryptedData.From(folder.EncryptedKey), 
            withRootKey ? EncryptedData.From(folder.KeyEncryptedByRoot) : null,
            folder.ParentFolderId,
            children,
            folder.CreatedAt);
    }
};