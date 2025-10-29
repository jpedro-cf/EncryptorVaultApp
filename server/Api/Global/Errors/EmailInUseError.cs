namespace EncryptionApp.Api.Global.Errors;

public class EmailInUseError(string message): AppError(message, StatusCodes.Status409Conflict, "Email In Use Error") ;