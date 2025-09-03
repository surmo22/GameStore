using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace GameStore.WebApi.Middlewares.Loggers;

public class RequestLogger(ILogger<RequestLogger> logger) : IRequestLogger
{
    public async Task LogRequestAsync(HttpContext context)
    {
        var fullRequestUrl = context.Request.GetDisplayUrl();
        var ipAddress = context.Connection.RemoteIpAddress;
        var method = context.Request.Method;
        context.Request.EnableBuffering();

        var requestBodyReader = new StreamReader(context.Request.Body);
        var requestBodyContent = await requestBodyReader.ReadToEndAsync();
        if (requestBodyContent.IsNullOrEmpty())
        {
            requestBodyContent = "No content";
        }

        context.Request.Body.Position = 0;

        logger.LogInformation(
            "Incoming request from {IpAddress}, Method = {Method}. URL: {FullRequestUrl}, Content: {RequestBodyContent}",
            ipAddress,
            method,
            fullRequestUrl,
            requestBodyContent);
    }
}