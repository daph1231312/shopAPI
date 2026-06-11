using System.Net;
using System.Text.Json;

namespace ShopAPI.Middleware;

/// <summary>
/// Global error handling middleware.
/// Catches unhandled exceptions and returns consistent JSON error responses.
/// Without this, unhandled exceptions return ugly HTML stack traces to clients.
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    /// <summary>Requires the next middleware in the pipeline and a logger.</summary>
    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>Invokes the middleware, catching and transforming any unhandled exceptions.</summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var result = JsonSerializer.Serialize(new
        {
            error = "An unexpected error occurred.",
            detail = exception.Message
        });

        return context.Response.WriteAsync(result);
    }
}
