using System.ComponentModel.DataAnnotations;

namespace MyMVCProject.Api.Entities;

public class BaseEncryptedEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required] 
    public string Name { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public string EncryptedKey { get; set; }
    public string KeyEncryptedByRoot { get; set; }
    
    public string RootKeySalt { get; set; }
    
    [Required]
    public Guid OwnerId { get; set; }
    public User Owner { get; set; }
}