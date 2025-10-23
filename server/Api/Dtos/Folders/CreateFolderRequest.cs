using System.ComponentModel.DataAnnotations;

namespace EncryptionApp.Api.Dtos.Folders;

public class CreateFolderRequest
{
    [Required(ErrorMessage = "Folder name is required.")]
    public string Name { get; set; }
    
    public Guid? ParentId { get; set; }
    
    [Required(ErrorMessage = "Encrypted key is required.")]
    [Base64String]
    public string EncryptedKey { get; set; }
    [Required(ErrorMessage = "Key encrypted by root is required.")]
    [Base64String]
    public string KeyEncryptedByRoot { get; set; }
    
}