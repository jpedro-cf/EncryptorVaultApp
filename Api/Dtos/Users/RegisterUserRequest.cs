using System.ComponentModel.DataAnnotations;

namespace MyMVCProject.Api.Dtos.Users;

public class RegisterUserRequest
{   
    [Required(ErrorMessage = "E-mail is required")]
    [EmailAddress(ErrorMessage = "Invalid e-mail.")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "Password confirmation is required.")]
    [Compare(nameof(Password), ErrorMessage = "The passwords are not the same.")]
    public string PasswordConfirmation { get; set; }
}