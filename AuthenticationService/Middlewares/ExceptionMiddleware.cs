using System.Diagnostics;
using System.Text.Json;

namespace AuthenticationService.Middlewares;

public sealed class ExceptionMiddleware(
    ILogger<ExceptionMiddleware> logger,
    IHostEnvironment environment) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var requestId = Activity.Current?.Id ?? context.TraceIdentifier;
            logger.LogError(exception, "Unhandled exception {RequestId} at {Path}", requestId, context.Request.Path);

            if (context.Response.HasStarted)
                throw;

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                requestId,
                title = "An unexpected error occurred.",
                status = StatusCodes.Status500InternalServerError,
                detail = environment.IsDevelopment() ? exception.Message : null,
                path = context.Request.Path.Value
            }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        }
    }
}
