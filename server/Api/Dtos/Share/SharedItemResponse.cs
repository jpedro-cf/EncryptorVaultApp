using System.Text.Json.Serialization;
using EncryptionApp.Api.Dtos.Items;
using EncryptionApp.Api.Entities;

namespace EncryptionApp.Api.Dtos.Share;

public record SharedItemResponse(
    Guid Id, 
    Guid ItemId, 
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    ItemType Type, 
    EncryptedData EncryptedKey, 
    DateTime CreatedAt)
{
    public static SharedItemResponse From(SharedItem item)
    {
        return new SharedItemResponse(item.Id, item.ItemId, item.ItemType, EncryptedData.From(item.EncryptedKey), item.CreatedAt);
    }
}