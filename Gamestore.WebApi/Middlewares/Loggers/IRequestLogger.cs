namespace GameStore.WebApi.Middlewares.Loggers;

public interface IRequestLogger
{
    Task LogRequestAsync(HttpContext context);
}