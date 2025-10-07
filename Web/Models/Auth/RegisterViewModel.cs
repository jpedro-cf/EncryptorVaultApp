using MyMVCProject.Api.Dtos.Users;

namespace MyMVCProject.Web.Models.Auth;

public class RegisterViewModel: RegisterUserRequest
{
    public string? ErrorMessage {get; set; }
}