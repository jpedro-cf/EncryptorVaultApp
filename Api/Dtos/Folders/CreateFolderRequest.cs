using System.ComponentModel.DataAnnotations;

namespace MyMVCProject.Api.Dtos.Folders;

public class CreateFolderRequest
{
    [Required(ErrorMessage = "Folder name is required.")]
    public string Name { get; set; }
    
    public Guid? ParentId { get; set; }
    
    [Required(ErrorMessage = "Encryption key is required.")]
    public string EncryptionKey { get; set; }
    [Required(ErrorMessage = "Root encryption key is required.")]
    public string RootEncryptionKey { get; set; }
    
}