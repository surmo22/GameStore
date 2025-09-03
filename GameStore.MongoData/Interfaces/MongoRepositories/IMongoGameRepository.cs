using GameStore.BLL.DTOs.Games;
using GameStore.BLL.GamesQueryPipeline.Interfaces;
using GameStore.Domain.MongoEntities;

namespace GameStore.MongoData.Interfaces.MongoRepositories;

public interface IMongoGameRepository
{
    Task<GameList<MongoGame>> GetAllGamesAsync(IGamePipeline gamePipeline, CancellationToken cancellationToken);
    Task<MongoGame?> GetGameByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<MongoGame?> GetGameByKeyAsync(string key, CancellationToken cancellationToken);
    Task<bool> GameExistsAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateUnitsInStockAsync(Guid id, int delta, CancellationToken cancellationToken);
    Task IncrementViewCountAsync(Guid id, CancellationToken cancellationToken);
}