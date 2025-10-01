using Microsoft.AspNetCore.Identity;

namespace MyMVCProject.Api.Entities;

public class User: IdentityUser<Guid>
{
    public ICollection<Folder> Folders { get; set; }
    public ICollection<File> Files { get; set; }
}