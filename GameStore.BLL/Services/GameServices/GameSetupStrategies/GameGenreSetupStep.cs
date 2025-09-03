using GameStore.BLL.DTOs;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.BLL.Interfaces.GameSetupStrategies;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Services.GameServices.GameSetupStrategies;

public class GameGenreSetupStep(IGenreService genreService) : IAsyncGameSetupStep
{
    public async Task InitializeFieldAsync(Game game, IGameRequest gameRequest, CancellationToken cancellationToken)
    {
        if (gameRequest.Genres is { Count: not 0 })
        {
            game.Genres = await genreService.GetGenresByIdsAsync(gameRequest.Genres, cancellationToken);
        }
    }
}