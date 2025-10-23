namespace EncryptionApp.Api.Global.Errors;

public class TwoFactorRequiredError(string message) 
    : AppError(message, StatusCodes.Status401Unauthorized, "TwoFactorAuthenticationRequired");
