namespace MyMVCProject.Api.Global.Exceptions;

public class UnauthorizedException(string message)
    : ApplicationException(message, StatusCodes.Status401Unauthorized, "Unauthorized.");