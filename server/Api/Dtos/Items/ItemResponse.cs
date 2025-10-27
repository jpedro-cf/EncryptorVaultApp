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
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    ContentType? ContentType,
    string? Url,
    DateTime CreatedAt,
    Guid? ParentId,
    EncryptedData EncryptedKey,
    EncryptedData? KeyEncryptedByRoot)
{
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