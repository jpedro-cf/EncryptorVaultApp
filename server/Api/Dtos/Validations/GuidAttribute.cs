using System.ComponentModel.DataAnnotations;

namespace EncryptionApp.Api.Dtos.Validations;

public class GuidAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null) return ValidationResult.Success;

        if (Guid.TryParse(value.ToString(), out _))
            return ValidationResult.Success;

        return new ValidationResult($"{validationContext.DisplayName} must be a valid GUID.");
    }
}