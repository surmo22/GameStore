namespace GameStore.WebApi.Filters.Handlers;

public class UnauthorizedAccessExceptionHandler : IExceptionHandler
{
    public async Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger logger,
        CancellationToken cancellationToken)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(exception.Message, cancellationToken);
    }

    public bool CanHandle(Exception exception)
    {
        return exception is UnauthorizedAccessException;
    }
}