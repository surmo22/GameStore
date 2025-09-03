namespace GameStore.WebApi.Middlewares.Loggers;

public interface IResponseLogger
{
    void LogResponse(string url, int statusCode, byte[] snippet);
}