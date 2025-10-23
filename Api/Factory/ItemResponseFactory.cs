using EncryptionApp.Api.Dtos.Folders;
using EncryptionApp.Api.Dtos.Items;
using EncryptionApp.Api.Entities;
using EncryptionApp.Api.Infra.Storage;
using File = EncryptionApp.Api.Entities.File;

namespace EncryptionApp.Api.Factory;

public class ItemResponseFactory(AmazonS3 amazonS3)
{
    public async Task<ItemResponse> CreateFrom(File file, bool withRootKey)
    {
        var presignedUrl = await amazonS3.GeneratePresignedUrl(file.StorageKey);
        
        return new ItemResponse(
            file.Id,
            ItemType.File,
            EncryptedData.From(file.Name), 
            file.Size,
            file.ContentType,
            presignedUrl,
            file.CreatedAt,
            file.ParentFolderId,
            EncryptedData.From(file.EncryptedKey), 
            withRootKey ? EncryptedData.From(file.KeyEncryptedByRoot) : null);
    }

    public async Task<FolderResponse> CreateFolderResponse(Folder folder, bool withRootKey)
    {
        var children = folder.Folders
            .Select(f => ItemResponse.From(f, withRootKey))
            .ToList() ?? [];
        
        var subItems = await Task.WhenAll(
            folder.Files.Select(f => CreateFrom(f, withRootKey))
        );
        
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
}