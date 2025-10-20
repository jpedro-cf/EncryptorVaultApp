using Microsoft.AspNetCore.Identity;

namespace EncryptionApp.Api.Entities;

public class User: IdentityUser<Guid>
{
    // VaultKey: [salt | iv | encrypted data for example]
    // It's used to verify if the user provided a correct key for decryption of the entire vault
    // Generates an irrelevant random data and encrypt it, client-side will try to decrypt it to validate
    public string? VaultKey { get; set; }
    public virtual ICollection<Folder> Folders { get; set; }
    public virtual ICollection<File> Files { get; set; }
    public virtual ICollection<StorageUsage> StorageUsages { get; set; }
}