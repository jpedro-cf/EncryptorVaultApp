namespace MyMVCProject.Api.Dtos.Folders;

public record CreateFolderResponse(FolderResponse folder, string decryptedKey, string decryptedRootKey);