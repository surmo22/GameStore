using GameStore.BLL.Interfaces.GameServices;

namespace GameStore.WebApi.Middlewares;

public class GamesCountMiddleware(RequestDelegate next, IGameCountService gameCountServiceService)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var totalGames = gameCountServiceService.GetTotalGamesCount();
        context.Response.Headers.Append("x-total-numbers-of-games", totalGames.ToString());
        await next(context);
    }
}