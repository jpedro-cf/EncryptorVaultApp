using System.ComponentModel.DataAnnotations;

namespace EncryptionApp.Api.Dtos.Users;

public class UpdateAccountRequest
{
    [EmailAddress]
    public string? Email { get; set; }
    
    [Required(ErrorMessage = "Current Password is required.")]
    public string CurrentPassword { get; set; }
    
    [PasswordValidation(
        MinimumLength = 8,
        RequireDigit = true,
        RequireLowercase = true,
        RequireUppercase = true,
        RequireNonAlphanumeric = true,
        RequiredUniqueChars = 1)]
    public string? NewPassword { get; set; }
}