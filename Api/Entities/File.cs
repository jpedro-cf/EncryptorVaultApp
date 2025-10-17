using System.ComponentModel.DataAnnotations;

namespace MyMVCProject.Api.Entities;

public class File : BaseEncryptedEntity
{
    [Required]
    [MaxLength(256)]
    public string StorageKey { get; set; }

    [Required] 
    public FileStatus Status { get; set; } = FileStatus.Pending;
    
    [Required]
    public string ContentType { get; set; }
    
    [Required]
    public long Size { get; set; }
    
    public Guid? ParentFolderId { get; set; }
    public Folder? ParentFolder { get; set; }
}

public enum FileStatus
{
    Failed,
    Pending,
    Completed
}