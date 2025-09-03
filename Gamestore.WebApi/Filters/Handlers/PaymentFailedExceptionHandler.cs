using GameStore.Common.Exceptions;

namespace GameStore.WebApi.Filters.Handlers;

public class PaymentFailedExceptionHandler : IExceptionHandler
{
    public async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";
        logger.LogError(exception, "Payment failed");
        await context.Response.WriteAsync("Payment failed", cancellationToken);
    }

    public bool CanHandle(Exception exception)
    {
        return exception is PaymentFailedException;
    }
}