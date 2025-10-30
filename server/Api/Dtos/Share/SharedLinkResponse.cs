using System.Text.Json.Serialization;
using EncryptionApp.Api.Dtos.Items;
using EncryptionApp.Api.Entities;

namespace EncryptionApp.Api.Dtos.Share;

public record SharedLinkResponse(
    Guid Id, 
    Guid ItemId, 
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    ItemType Type, 
    EncryptedData EncryptedKey, 
    DateTime CreatedAt)
{
    public static SharedLinkResponse From(SharedLink link)
    {
        return new SharedLinkResponse(link.Id, link.ItemId, link.ItemType, EncryptedData.From(link.EncryptedKey), link.CreatedAt);
    }
}