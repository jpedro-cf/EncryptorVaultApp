namespace MyMVCProject.Api.Dtos.Users;

public record MfaKeyResponse(string Key, string QrCodeBase64);