namespace EncryptionApp.Api.Dtos.Users;

public record LoginResponse(UserResponse User, string Token, bool RequiresTwoFactor);