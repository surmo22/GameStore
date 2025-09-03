namespace GameStore.WebApi.Filters.Handlers;

public class KeyNotFoundExceptionHandler : IExceptionHandler
{
    public async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling exception {Exception}.", exception.GetType().Name);
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("Requested entity was not found", cancellationToken);
    }

    public bool CanHandle(Exception exception)
    {
        return exception is KeyNotFoundException;
    }
}