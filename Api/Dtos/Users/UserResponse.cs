using EncryptionApp.Api.Dtos.Items;
using EncryptionApp.Api.Entities;
using Microsoft.IdentityModel.Tokens;

namespace EncryptionApp.Api.Dtos.Users;

public record UserResponse(string Id, string Email, string Username, EncryptedData? EncryptedVaultKey)
{
    public static UserResponse From(User user)
    {
        return new UserResponse(
            user.Id.ToString(),
            user.Email!,
            user.Email!,
            user.VaultKey.IsNullOrEmpty() ? null : EncryptedData.From(user.VaultKey!)
        );
    }
}