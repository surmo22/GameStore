using System.Text;

namespace GameStore.WebApi.Middlewares.Loggers;

public class ResponseLogger(ILogger<ResponseLogger> logger) : IResponseLogger
{
    public void LogResponse(string url, int statusCode, byte[] snippet)
    {
        try
        {
            int len = Array.IndexOf(snippet, (byte)0);
            if (len == -1) len = snippet.Length;
            var snippetString = Encoding.UTF8.GetString(snippet, 0, len);
            var message = $"Response to {url} with status code {statusCode}. Body: {snippetString}...";
            logger.LogInformation(message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while logging response");
        }
    }
}