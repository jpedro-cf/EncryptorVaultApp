using System.ComponentModel.DataAnnotations;

namespace MyMVCProject.Api.Dtos.Files;

public class UploadFileRequest
{
    [Required(ErrorMessage = "File name is required.")]
    public string FileName { get; set; }
    
    [Required(ErrorMessage = "File size is required")]
    public long FileSize { get; set; }
    
    [Required(ErrorMessage = "Encryption key is requried.")]
    public string EncryptionKey { get; set; }
    [Required(ErrorMessage = "Root encryption key is requried.")]
    public string RootEncryptionKey { get; set; }

    public Guid? ParentFolderId { get; set; }
}

public record InitiateUploadResponse(string UploadId, string Key, List<PresignedPartUrl> Urls);

public record PresignedPartUrl(int PartNumber, string Url);

public class CompletedPart
{
    [Required]
    public int PartNumber { get; set; }
    
    [Required]
    public string ETag { get; set; }
};

public class CompleteUploadRequest
{
    [Required(ErrorMessage = "Upload id is required.")]
    public string UploadId { get; set; }
    
    [Required(ErrorMessage = "The object key is required.")]
    public string Key { get; set; }

    [Required(ErrorMessage = "CompletedParts is required.")]
    public List<CompletedPart> Parts { get; set; }
}

public record UploadCompletedResponse(string Key);