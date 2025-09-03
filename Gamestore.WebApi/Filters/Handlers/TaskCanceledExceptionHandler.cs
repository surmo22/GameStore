namespace GameStore.WebApi.Filters.Handlers;

public class TaskCanceledExceptionHandler : IExceptionHandler
{
    public async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling task cancellation");
        context.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("Task was aborted", cancellationToken);
    }

    public bool CanHandle(Exception exception)
    {
        return exception is TaskCanceledException;
    }
}