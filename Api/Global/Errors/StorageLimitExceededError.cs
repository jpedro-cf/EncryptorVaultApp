namespace MyMVCProject.Api.Global.Errors;

public class StorageLimitExceededError(string message): 
    AppError(message, StatusCodes.Status507InsufficientStorage, "StorageLimitExceeded");