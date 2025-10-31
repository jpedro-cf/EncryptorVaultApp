namespace EncryptionApp.Api.Workers;

public record BackgroundTask(Guid Id, BackgroundTaskType Type);

public enum BackgroundTaskType
{
    User,
    Folder,
    File
}