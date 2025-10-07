using Microsoft.AspNetCore.Mvc;

namespace MyMVCProject.Api.Global.Errors;

public class AppError(string message, int statusCode, string? title)
{
    public string Title { get; } = title ?? "Internal Server Error.";
    public int StatusCode { get; } = statusCode;
    public string Message { get; } = message;
    
    public ProblemDetails ToProblemDetail()
    {
        return new ProblemDetails
        {
            Title = Title,
            Detail = Message,
            Status = StatusCode
        };
    }

    public IResult ToHttpResult()
    {
        return Results.Problem(ToProblemDetail());
    }
}