using GameStore.Common.Exceptions;

namespace GameStore.WebApi.Filters.Handlers;

public class InvalidGenreHierarchyExceptionHandler : IExceptionHandler
{
    public async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling exception {Exception}.", exception.GetType().Name);
        context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(exception.Message, cancellationToken);
    }

    public bool CanHandle(Exception exception)
    {
        return exception is InvalidGenreHierarchyException;
    }
}