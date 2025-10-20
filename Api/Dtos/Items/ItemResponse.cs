using System.Text.Json.Serialization;
using EncryptionApp.Api.Entities;
using File = EncryptionApp.Api.Entities.File;

namespace EncryptionApp.Api.Dtos.Items;

public record ItemResponse(
    Guid Id,
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    ItemType Type,
    EncryptedData EncryptedName,
    long? Size,
    string? ContentType,
    string? StorageKey,
    DateTime CreatedAt,
    Guid? ParentId,
    EncryptedData EncryptedKey,
    EncryptedData? KeyEncryptedByRoot)
{
    public static ItemResponse From(File file, bool withRootKey)
    {
        return new ItemResponse(
            file.Id,
            ItemType.File,
            EncryptedData.From(file.Name), 
            file.Size,
            file.ContentType,
            file.StorageKey,
            file.CreatedAt,
            file.ParentFolderId,
            EncryptedData.From(file.EncryptedKey), 
            withRootKey ? EncryptedData.From(file.KeyEncryptedByRoot) : null);
    }
    
    public static ItemResponse From(Folder folder, bool withRootKey)
    {
        return new ItemResponse(
            folder.Id,
            ItemType.Folder,
            EncryptedData.From(folder.Name), 
            null,
            null,
            null,
            folder.CreatedAt,
            folder.ParentFolderId,
            EncryptedData.From(folder.EncryptedKey), 
            withRootKey ? EncryptedData.From(folder.KeyEncryptedByRoot) : null);
    }
}