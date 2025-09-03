using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;

namespace GameStore.MongoData.Interfaces.GameDependencyResolvers;

public interface IMongoGameDependenciesResolver
{
    Task<Game> ResolveGameDependenciesAsync(Game game, MongoGame mongoGame, CancellationToken cancellationToken);
}