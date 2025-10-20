using EncryptionApp.Api.Entities;

namespace EncryptionApp.Api.Dtos.Users;

public record UserResponse(string Id, string Email, string Username, string? VaultKey)
{
    public static UserResponse From(User user)
    {
        return new UserResponse(
            user.Id.ToString(),
            user.Email!,
            user.Email!,
            user.VaultKey
        );
    }
}