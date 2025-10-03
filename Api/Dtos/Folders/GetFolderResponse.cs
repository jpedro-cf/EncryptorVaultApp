namespace MyMVCProject.Api.Dtos.Folders;

public record GetFolderResponse(FolderResponse Folder, string DecryptionKey, string? RootDecryptionKey);