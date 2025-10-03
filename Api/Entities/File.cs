using System.ComponentModel.DataAnnotations;

namespace MyMVCProject.Api.Entities;

public class File : BaseEncryptedEntity
{
    [Required]
    [MaxLength(256)]
    public string StorageKey { get; set; }
    
    [Required]
    public long Size { get; set; }
    
    [Required]
    public Guid ParentFolderId { get; set; }
    public Folder ParentFolder { get; set; }
}