using MyMVCProject.Api.Dtos.Users;

namespace MyMVCProject.Web.Models.Auth;

public class LoginViewModel: LoginRequest
{
    public string? ErrorMessage { get; set; }
}