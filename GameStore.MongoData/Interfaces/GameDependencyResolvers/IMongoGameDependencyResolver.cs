using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;

namespace GameStore.MongoData.Interfaces.GameDependencyResolvers;

public interface IMongoGameDependencyResolver
{
    Task ResolveDependency(Game game, MongoGame mongoGame, CancellationToken cancellationToken);
}