namespace MyMVCProject.Api.Global.Exceptions;

public class ForbiddenException(string message) 
    : ApplicationException(message, StatusCodes.Status403Forbidden, "Forbidden Request.");