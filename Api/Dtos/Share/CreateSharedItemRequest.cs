using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using EncryptionApp.Api.Entities;

namespace EncryptionApp.Api.Dtos.Share;

public class CreateSharedItemRequest
{
    [Required]
    public Guid ItemId { get; set; }
    
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ItemType Type { get; set; }
    
    [Required]
    [Base64String]
    public string EncryptedKey { get; set; }
}