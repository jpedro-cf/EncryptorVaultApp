using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EncryptionApp.Api.Entities;

public class User: IdentityUser<Guid>
{
    // VaultKey: [salt | iv | encrypted root key]
    // The root key is encrypted by a key that's derived by some sort of "secret code" provided by the user
    // If the user decides to change this secret, we just encrypt it again with the new key
    // In this way, the server has no idea of the root key
    public string? VaultKey { get; set; }
    
    public virtual ICollection<Folder> Folders { get; set; }
    public virtual ICollection<File> Files { get; set; }
    public virtual ICollection<StorageUsage> StorageUsages { get; set; }
    public virtual ICollection<SharedLink> SharedLinks { get; set; }
}