namespace EncryptionApp.Api.Dtos.Users;

public record MfaKeyResponse(string Key, string QrCodeBase64);