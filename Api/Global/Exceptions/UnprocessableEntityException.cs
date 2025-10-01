namespace MyMVCProject.Api.Global.Exceptions;

public class UnprocessableEntityException (string message)
    : ApplicationException(message, StatusCodes.Status422UnprocessableEntity, "Unprocessable Entity");