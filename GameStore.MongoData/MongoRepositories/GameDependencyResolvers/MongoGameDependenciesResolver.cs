using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;
using GameStore.MongoData.Interfaces.GameDependencyResolvers;

namespace GameStore.MongoData.MongoRepositories.GameDependencyResolvers;

public class MongoGameDependenciesResolver(IEnumerable<IMongoGameDependencyResolver> resolvers) : IMongoGameDependenciesResolver
{
    public async Task<Game> ResolveGameDependenciesAsync(Game game, MongoGame mongoGame, CancellationToken cancellationToken)
    {
        foreach (var resolver in resolvers)
        {
            await resolver.ResolveDependency(game, mongoGame, cancellationToken);
        }

        return game;
    }
}