using System.Data;

namespace GameStore.WebApi.Filters.Handlers;

public class DbConcurrencyExceptionHandler : IExceptionHandler
{
    public async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling exception {Exception}.", exception.GetType().Name);
        context.Response.StatusCode = StatusCodes.Status409Conflict;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("The data has been modified by another user", cancellationToken);
    }

    public bool CanHandle(Exception exception)
    {
        return exception is DBConcurrencyException;
    }
}