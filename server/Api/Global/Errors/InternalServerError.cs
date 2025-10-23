namespace EncryptionApp.Api.Global.Errors;

public class InternalServerError(string message) 
    : AppError(message, StatusCodes.Status500InternalServerError, "Internal Server Error");