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
    
    // [ salt | iv | ciphertext ]
    [Required]
    public string KeyEncryptedByRoot { get; set; }
    
    [Required]
    public Guid OwnerId { get; set; }
    public User Owner { get; set; }

    public EncryptionKey GetEncryptedKey()
    {
        var combined = Convert.FromBase64String(EncryptedKey);

        // Extract IV and data
        var iv = combined[..16];
        var encryptedData = combined[16..];

        return new EncryptionKey(encryptedData.ToBase64(), iv.ToBase64(), null);
    }
    
    public EncryptionKey GetRootKey()
    {
        // [ salt | iv | ciphertext ]
        var combined = Convert.FromBase64String(KeyEncryptedByRoot);

        // Extract IV, salt and data
        var salt = combined[..16];
        var iv = combined[16..32];
        var encryptedData = combined[32..];

        return new EncryptionKey(encryptedData.ToBase64(), iv.ToBase64(), salt.ToBase64());
    }
}