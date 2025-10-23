namespace EncryptionApp.Api.Global.Errors;

public class UnauthorizedError(string message)
    : AppError(message, StatusCodes.Status401Unauthorized, "Unauthorized.");