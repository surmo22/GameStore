using System.Net;
using GameStore.WebApi.Filters.Handlers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GameStore.WebApi.Filters;

public class ExceptionFilter(ILogger<ExceptionFilter> logger, IEnumerable<IExceptionHandler> handlers) : IAsyncExceptionFilter
{
    public async Task OnExceptionAsync(ExceptionContext context)
    {
        var exception = context.Exception;
        var handler = handlers.FirstOrDefault(h => h.CanHandle(exception));

        if (handler is not null)
        {
            await handler.HandleExceptionAsync(context.HttpContext, exception, logger, context.HttpContext.RequestAborted);
            logger.LogInformation("The exception was handled successfully by {Handler}.", handler.GetType().Name);
        }
        else
        {
            logger.LogError(exception, "An unhandled exception has occured while processing request");

            await context.HttpContext.Response.WriteAsync(
                "An unknown error has occured while processing your request. Please try again later.",
                context.HttpContext.RequestAborted);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }

        context.ExceptionHandled = true;
    }
}
