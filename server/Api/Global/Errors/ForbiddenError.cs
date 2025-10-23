namespace EncryptionApp.Api.Global.Errors;

public class ForbiddenError(string message) 
    : AppError(message, StatusCodes.Status403Forbidden, "Forbidden Request.");