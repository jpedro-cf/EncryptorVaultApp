namespace EncryptionApp.Api.Global.Errors;

public class NotFoundError(string message) 
    : AppError(message, StatusCodes.Status404NotFound , "Not Found.");