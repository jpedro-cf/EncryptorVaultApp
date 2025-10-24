using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EncryptionApp.Api.Entities;

public class StorageUsage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
    
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ContentType ContentType { get; set; }
    
    [Required]
    public long TotalSize { get; set; }

}

