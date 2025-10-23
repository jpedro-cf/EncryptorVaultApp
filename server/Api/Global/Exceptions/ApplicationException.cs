using Microsoft.AspNetCore.Mvc;

namespace EncryptionApp.Api.Global.Exceptions;

[Serializable]
public class ApplicationException(string message, int? statusCode, string? title) : Exception(message)
{
    public int StatusCode { get; } = statusCode ?? StatusCodes.Status500InternalServerError;
    public string Title { get; } = title ?? "Internal Server Error.";

    public ProblemDetails ToProblemDetail()
    {
        return new ProblemDetails
        {
            Title = Title,
            Detail = Message,
            Status = StatusCode
        };
    }
}