namespace GameStore.WebApi.Filters.Handlers;

public class IoExceptionHandler : IExceptionHandler
{
    public async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling exception {Exception}.", exception.GetType().Name);
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("Could not create game file, try again later", cancellationToken);
    }

    public bool CanHandle(Exception exception)
    {
        return exception is IOException;
    }
}