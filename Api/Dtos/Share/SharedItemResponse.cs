using EncryptionApp.Api.Entities;

namespace EncryptionApp.Api.Dtos.Share;

public record SharedItemResponse(Guid Id, ItemType Type, string Key, DateTime CreatedAt)
{
    public static SharedItemResponse From(SharedItem item)
    {
        return new SharedItemResponse(item.Id, item.ItemType, item.EncryptedKey, item.CreatedAt);
    }
}