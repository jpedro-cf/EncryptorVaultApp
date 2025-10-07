using System.ComponentModel.DataAnnotations;

namespace MyMVCProject.Api.Dtos.Folders;

public class CreateFolderRequest
{
    [Required(ErrorMessage = "Folder name is required.")]
    public string Name { get; set; }
    
    public Guid? ParentId { get; set; }
    
    [Required(ErrorMessage = "Encrypted key is required.")]
    public string EncryptedKey { get; set; }
    [Required(ErrorMessage = "Key encrypted by root is required.")]
    public string KeyEncryptedByRoot { get; set; }
    
}