namespace MyMVCProject.Api.Global.Errors;

public class InvalidParamError(string message)
    : AppError(message, StatusCodes.Status400BadRequest, "Invalid param(s)");