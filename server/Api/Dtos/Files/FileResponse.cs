using EncryptionApp.Api.Dtos.Items;
using File = EncryptionApp.Api.Entities.File;

namespace EncryptionApp.Api.Dtos.Files;

public record FileResponse(
    Guid Id,
    EncryptedData EncryptedName,
    string StorageKey,
    long Size,
    string ContentType,
    string Url,
    EncryptedData EncryptedKey,
    EncryptedData? KeyEncryptedByRoot,
    Guid? ParentId,
    DateTime CreatedAt);