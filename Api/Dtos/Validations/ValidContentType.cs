using System.ComponentModel.DataAnnotations;
using EncryptionApp.Api.Global.Helpers;

namespace EncryptionApp.Api.Dtos.Validations;

public class ValidContentType: ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var str = value as string;

        if (string.IsNullOrEmpty(str))
            return new ValidationResult(ErrorMessage ?? "Content type is required");

        if (!str.IsValidContentType())
        {
            return new ValidationResult(ErrorMessage ?? "Invalid content type."); 
        }
        
        return ValidationResult.Success; 
    }
}