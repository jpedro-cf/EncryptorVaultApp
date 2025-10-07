using Microsoft.AspNetCore.Identity;

namespace MyMVCProject.Api.Entities;

public class User: IdentityUser<Guid>
{
    public string? VaultKeySalt { get; set; }
    public ICollection<Folder> Folders { get; set; }
    public ICollection<File> Files { get; set; }
}