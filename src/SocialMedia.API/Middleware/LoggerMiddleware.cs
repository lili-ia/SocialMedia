namespace SocialMedia.Middleware;

public class LoggerMiddleware
{
    private RequestDelegate _next;
    private ILogger<LoggerMiddleware> _logger;

    public LoggerMiddleware(RequestDelegate next, ILogger<LoggerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        _logger.LogInformation($"Incoming request: {context.Request.Method} {context.Request.Path}");
        await _next(context);
        _logger.LogInformation($"Response status: {context.Response.StatusCode}");
    }
}