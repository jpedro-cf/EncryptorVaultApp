using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EncryptionApp.Api.Entities;

public class SharedItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ItemId { get; set; }
    
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ItemType ItemType { get; set; }
    
    // encrypted with random key generated for share purpose only.
    [Required]
    public string EncryptedKey { get; set; }
    
    [Required]
    public Guid OwnerId { get; set; }
    public virtual User Owner { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}