using System.ComponentModel.DataAnnotations;

namespace EncryptionApp.Api.Dtos.Users;

public class SetupMfaRequest
{
    [Required(ErrorMessage = "Token is required.")]
    public string Code { get; set; }
}