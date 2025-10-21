using Microsoft.AspNetCore.Identity;

namespace EncryptionApp.Api.Entities;

public class User: IdentityUser<Guid>
{
    // VaultKey: [iv | encrypted key]
    // It's encrypted by a key that's derived by the user's password on account creation
    // If the user changes it's password, we just encrypt it again with the new key
    public string? VaultKey { get; set; }
    public virtual ICollection<Folder> Folders { get; set; }
    public virtual ICollection<File> Files { get; set; }
    public virtual ICollection<StorageUsage> StorageUsages { get; set; }
    public virtual ICollection<SharedItem> SharedItems { get; set; }
}