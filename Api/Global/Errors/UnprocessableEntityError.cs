namespace EncryptionApp.Api.Global.Errors;

public class UnprocessableEntityError (string message)
    : AppError(message, StatusCodes.Status422UnprocessableEntity, "Unprocessable Entity");