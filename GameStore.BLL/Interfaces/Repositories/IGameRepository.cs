using GameStore.BLL.DTOs.Games;
using GameStore.BLL.GamesQueryPipeline.Interfaces;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Interfaces.Repositories;

public interface IGameRepository
{
    Task<GameList<Game>> GetAllGamesAsync(IGamePipeline gamePipeline, CancellationToken cancellationToken);

    Task<Game?> GetGameByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Game?> GetGameByKeyAsync(string key, CancellationToken cancellationToken);

    Task AddGameAsync(Game game, CancellationToken cancellationToken);

    Task UpdateGameAsync(Game game, CancellationToken cancellationToken);

    Task DeleteGameAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> GameExistsAsync(Guid id, CancellationToken cancellationToken);
    
    Task UpdateUnitsInStockAsync(Guid id, int delta, CancellationToken cancellationToken);
    
    Task IncrementViewCountAsync(Guid id, CancellationToken cancellationToken);
}
