using Microsoft.AspNetCore.Identity;

namespace MyMVCProject.Api.Entities;

public class User: IdentityUser<Guid>
{
    public string? VaultKeySalt { get; set; }
    public virtual ICollection<Folder> Folders { get; set; }
    public virtual ICollection<File> Files { get; set; }
    public virtual ICollection<StorageUsage> StorageUsages { get; set; }
}