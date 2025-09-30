namespace MyMVCProject.Api.Global.Exceptions;

public class NotFoundException(string message) 
    : ApplicationException(message, StatusCodes.Status404NotFound , "Not Found.");