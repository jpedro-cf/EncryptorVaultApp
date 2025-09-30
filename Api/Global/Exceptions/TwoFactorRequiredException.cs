namespace MyMVCProject.Api.Global.Exceptions;

public class TwoFactorRequiredException(string message) 
    : ApplicationException(message, StatusCodes.Status401Unauthorized, "TwoFactorAuthenticationRequired");
