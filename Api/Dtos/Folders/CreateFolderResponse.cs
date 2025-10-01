namespace MyMVCProject.Api.Dtos.Folders;

public record CreateFolderResponse(FolderResponse Folder, string DecryptionKey, string RootDecryptionKey);