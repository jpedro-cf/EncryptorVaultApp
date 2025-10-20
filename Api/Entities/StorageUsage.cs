using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MyMVCProject.Api.Global;
using ApplicationException = MyMVCProject.Api.Global.Exceptions.ApplicationException;

namespace MyMVCProject.Api.Entities;

public class StorageUsage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
    
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public StorageContentType ContentType { get; set; }
    
    [Required]
    public long TotalSize { get; set; }

}

public enum StorageContentType
{
    Text,
    Image,
    Video,
    Audio,
    Application
}