using System.ComponentModel.DataAnnotations;
using MyMVCProject.Api.Dtos;
using MyMVCProject.Api.Global;

namespace MyMVCProject.Api.Entities;

public class BaseEncryptedEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required] 
    public string Name { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // [ iv | ciphertext ]
    public string EncryptedKey { get; set; }
    
    // [ iv | ciphertext ]
    [Required]
    public string KeyEncryptedByRoot { get; set; }
    
    [Required]
    public Guid OwnerId { get; set; }
    public User Owner { get; set; }
}