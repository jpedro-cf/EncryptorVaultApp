using System.ComponentModel.DataAnnotations;

namespace MyMVCProject.Api.Dtos.Users;

public class SetupMfaRequest
{
    [Required(ErrorMessage = "Token is required.")]
    public string Token { get; set; }
}