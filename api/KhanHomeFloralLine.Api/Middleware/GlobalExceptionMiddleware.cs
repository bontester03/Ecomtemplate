using System.Net;
using System.Text.Json;
using KhanHomeFloralLine.Application.Common;

namespace KhanHomeFloralLine.Api.Middleware;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            await WriteErrorAsync(context, ex);
        }
    }

    private static Task WriteErrorAsync(HttpContext context, Exception ex)
    {
        var statusCode = ex switch
        {
            AppValidationException => HttpStatusCode.BadRequest,
            AppNotFoundException => HttpStatusCode.NotFound,
            _ => HttpStatusCode.InternalServerError
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var payload = JsonSerializer.Serialize(new
        {
            error = ex.Message,
            status = context.Response.StatusCode,
            traceId = context.TraceIdentifier
        });

        return context.Response.WriteAsync(payload);
    }
}

