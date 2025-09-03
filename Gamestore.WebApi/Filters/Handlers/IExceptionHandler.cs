namespace GameStore.WebApi.Filters.Handlers;

public interface IExceptionHandler
{
    Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger logger, CancellationToken cancellationToken);

    bool CanHandle(Exception exception);
}