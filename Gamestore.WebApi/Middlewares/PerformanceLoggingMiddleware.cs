namespace GameStore.WebApi.Middlewares;

public class PerformanceLoggingMiddleware(RequestDelegate next, ILogger<PerformanceLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var requestStartTime = DateTime.UtcNow;

        await next(context);

        var elapsedMs = (DateTime.UtcNow - requestStartTime).TotalMilliseconds;

        logger.LogInformation("Time taken: {Time}", elapsedMs);
    }
}
