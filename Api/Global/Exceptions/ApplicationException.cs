using Microsoft.AspNetCore.Mvc;

namespace MyMVCProject.Api.Global.Exceptions;

[Serializable]
public class ApplicationException(string message, int statusCode, string? title) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
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