using System.ComponentModel.DataAnnotations;

namespace MyMVCProject.Api.Dtos.Users;

public class LoginMfaRequest
{
    [Required(ErrorMessage = "Code is required.")]
    public string Code { get; set; }
}