using MyMVCProject.Api.Entities;

namespace MyMVCProject.Api.Dtos.Users;

public record UserResponse(string Id, string Email, string Username, string? VaultKeySalt)
{
    public static UserResponse From(User user)
    {
        return new UserResponse(
            user.Id.ToString(),
            user.Email!,
            user.Email!,
            user.VaultKeySalt
        );
    }
}