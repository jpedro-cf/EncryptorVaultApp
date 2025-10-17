namespace MyMVCProject.Api.Dtos;

public record EncryptionKey(string Key, string Iv, string? Salt);