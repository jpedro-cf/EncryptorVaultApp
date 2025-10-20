using System.ComponentModel.DataAnnotations;

namespace EncryptionApp.Api.Dtos.Users;

public class LoginRequest
{
    [Required(ErrorMessage = "E-mail is required.")]
    [EmailAddress(ErrorMessage = "Invalid e-mail.")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; }
}