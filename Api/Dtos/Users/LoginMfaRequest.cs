using System.ComponentModel.DataAnnotations;

namespace EncryptionApp.Api.Dtos.Users;

public class LoginMfaRequest
{
    [Required(ErrorMessage = "Code is required.")]
    public string Code { get; set; }
}