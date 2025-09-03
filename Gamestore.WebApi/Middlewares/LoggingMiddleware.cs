using GameStore.WebApi.Middlewares.Loggers;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.IO;

namespace GameStore.WebApi.Middlewares;

public class LoggingMiddleware(
    RequestDelegate next,
    IRequestLogger requestLogger,
    IResponseLogger responseLogger,
    RecyclableMemoryStreamManager factory)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var originalResponseBodyStream = context.Response.Body;
        await using var newResponseBodyStream = factory.GetStream();
        context.Response.Body = newResponseBodyStream;

        await requestLogger.LogRequestAsync(context);
        await next(context);

        newResponseBodyStream.Position = 0;
        await newResponseBodyStream.CopyToAsync(originalResponseBodyStream);
        context.Response.Body = originalResponseBodyStream;

        newResponseBodyStream.Position = 0;

        var buffer = new byte[2048];
        _ = await newResponseBodyStream.ReadAsync(buffer.AsMemory(), context.RequestAborted);
        var url = context.Request.GetDisplayUrl();
        var statusCode = context.Response.StatusCode;

        _ = Task.Run(() =>
            responseLogger.LogResponse(
                url,
                statusCode, 
                buffer
            ));
    }
}