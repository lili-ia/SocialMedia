using System.Data.Common;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace SocialMedia.Middleware;

public class GlobalExceptionHandlingMiddleware : IMiddleware
{
    private ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context); 
        }
        catch (DbException ex)
        {
            _logger.LogError(ex, "A database error occurred.");

            await WriteProblemDetailsAsync(context, HttpStatusCode.InternalServerError,
                "Database error", "A problem occurred while accessing the database.");
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt.");

            await WriteProblemDetailsAsync(context, HttpStatusCode.Unauthorized,
                "Unauthorized", "You are not authorized to access this resource.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");

            await WriteProblemDetailsAsync(context, HttpStatusCode.InternalServerError,
                "Server error", "An internal error has occurred.");
        }
    }

    private async Task WriteProblemDetailsAsync(HttpContext context, HttpStatusCode statusCode, string title, string detail)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var problem = new ProblemDetails
        {
            Status = (int)statusCode,
            Type = $"https://httpstatuses.com/{(int)statusCode}",
            Title = title,
            Detail = detail
        };

        var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}