using System.Text.Json.Serialization;
using EncryptionApp.Api.Dtos.Items;
using EncryptionApp.Api.Entities;

namespace EncryptionApp.Api.Dtos.Share;

public record SharedContentResponse(
    List<ItemResponse> Items,
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    ItemType ItemType, 
    EncryptedData KeyToDecryptItems);