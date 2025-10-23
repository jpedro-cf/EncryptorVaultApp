using EncryptionApp.Api.Dtos.Items;
using EncryptionApp.Api.Entities;

namespace EncryptionApp.Api.Dtos.Folders;

public record FolderResponse(
    Guid Id,
    EncryptedData EncryptedName,
    EncryptedData EncryptedKey,
    EncryptedData? KeyEncryptedByRoot,
    Guid? ParentId,
    List<ItemResponse> Children,
    DateTime CreatedAt);