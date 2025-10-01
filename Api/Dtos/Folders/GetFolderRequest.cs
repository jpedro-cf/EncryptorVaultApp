using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace MyMVCProject.Api.Dtos.Folders;

public class GetFolderRequest : IValidatableObject
{   
    [FromHeader(Name = "secret")]
    public string? Secret { get; set; }
    [FromHeader(Name = "encryptionKey")]
    public string? EncryptionKey { get; set; }
    [FromHeader(Name = "token")]
    public string? Token { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Secret == null && EncryptionKey == null && Token == null)
        {
            yield return new ValidationResult(
                "You must provide at least one of the following: a secret, an encryption key, or a token.",
                new[] { nameof(Secret), nameof(EncryptionKey), nameof(Token) }
            );
        }
    }
}