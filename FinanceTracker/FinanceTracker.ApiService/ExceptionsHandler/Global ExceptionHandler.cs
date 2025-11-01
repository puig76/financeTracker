using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.ApiService.ExceptionsHandler;

public class Global_ExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = exception switch
        {
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            ApplicationException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        return await problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Type = exception.GetType().ToString(),
                    Title = "error occurred",
                    Detail = exception.Message
                }
            });
    }
}
