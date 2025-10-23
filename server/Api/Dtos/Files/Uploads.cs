using System.ComponentModel.DataAnnotations;
using EncryptionApp.Api.Dtos.Validations;

namespace EncryptionApp.Api.Dtos.Files;

public class UploadFileRequest
{
    [Required(ErrorMessage = "File name is required.")]
    public string FileName { get; set; }
    
    [Required(ErrorMessage = "File size is required")]
    public long FileSize { get; set; }
    
    [Required(ErrorMessage = "Content type is required")]
    [ValidContentType]
    public string ContentType { get; set; }
    
    [Required(ErrorMessage = "Encrypted key is required.")]
    [Base64String]
    public string EncryptedKey { get; set; }
    [Required(ErrorMessage = "Key encrypted by root is required.")]
    [Base64String]
    public string KeyEncryptedByRoot { get; set; }

    public Guid? ParentFolderId { get; set; }
}

public record InitiateUploadResponse(string FileId, string UploadId, string Key, List<PresignedPartUrl> Urls);

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
    [Required(ErrorMessage = "File ID is required.")]
    public string FileId { get; set; }
    
    [Required(ErrorMessage = "Upload id is required.")]
    public string UploadId { get; set; }
    
    [Required(ErrorMessage = "The object key is required.")]
    public string Key { get; set; }

    [Required(ErrorMessage = "CompletedParts is required.")]
    public List<CompletedPart> Parts { get; set; }
}

public record UploadCompletedResponse(string Key);

public record UploadPart(DateTime? LastModified, long? Size, int? PartNumber, string? ETag);

public record CancelUploadRequest(string Key, string UploadId);