using System.ComponentModel.DataAnnotations;

namespace MyMVCProject.Api.Dtos.Folders;

public class CreateFolderRequest : IValidatableObject
{
    [Required(ErrorMessage = "Folder name is required.")]
    public string Name { get; set; }
    
    public Guid? ParentId { get; set; }
    
    public string? Secret { get; set; }
    
    public string? EncryptionKey { get; set; }
    public string? RootEncryptionKey { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ParentId == null && string.IsNullOrWhiteSpace(Secret))
        {
            yield return new ValidationResult(
                "Secret is required for root folders.",
                new[] { nameof(Secret) }
            );
        }
        
        if (ParentId != null &&
            (string.IsNullOrWhiteSpace(RootEncryptionKey) || string.IsNullOrWhiteSpace(EncryptionKey)))
        {
            yield return new ValidationResult(
                "Subfolders require both a Root Key and an Encryption Key.",
                new[] { nameof(RootEncryptionKey), nameof(EncryptionKey) }
            );
        }
    }
}