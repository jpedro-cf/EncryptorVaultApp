namespace MyMVCProject.Api.Global.Exceptions;

public class InvalidParamException(string message)
    : ApplicationException(message, StatusCodes.Status400BadRequest, "Invalid param(s)");