using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EncryptionApp.Api.Global.Exceptions;

public class GlobalExceptionHandler(IProblemDetailsService problemDetailsService): IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        var genericProblem = new ProblemDetails
        {
            Title = "Internal Server Error",
            Detail = "An error occurred while performing this operation. Try again later or contact us.",
            Status = StatusCodes.Status500InternalServerError,
        };

        httpContext.Response.StatusCode = exception switch
        {
            ApplicationException appEx => appEx.StatusCode,
            _ => StatusCodes.Status500InternalServerError
        };
        
        if (httpContext.Request.Path.StartsWithSegments("/api") 
            || httpContext.Request.Headers["Accept"].ToString().Contains("application/json"))
        {
            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = exception switch
                {
                    ApplicationException appEx => appEx.ToProblemDetail(),
                    _ => genericProblem
                }
            });
        }
        
        httpContext.Response.Redirect("/Home/Error");
        return true;
    }
}