using EncryptionApp.Api.Entities;

namespace EncryptionApp.Api.Dtos.Users;

public record CurrentUserResponse(UserResponse User, Dictionary<ContentType, long> StorageUsage);