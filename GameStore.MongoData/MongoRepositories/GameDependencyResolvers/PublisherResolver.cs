using AutoMapper;
using GameStore.Data.Data;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;
using GameStore.MongoData.Interfaces.GameDependencyResolvers;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace GameStore.MongoData.MongoRepositories.GameDependencyResolvers;

public class PublisherResolver(NorthwindMongoContext context, GameStoreContext sqlContext, IMapper mapper) : IMongoGameDependencyResolver
{
    public async Task ResolveDependency(Game game, MongoGame mongoGame, CancellationToken cancellationToken)
    {
        if (mongoGame.SupplierId > 0)
        {
            var mongoPublisher = await context.Publishers.AsQueryable()
                .Where(g => g.SupplierId == mongoGame.SupplierId)
                .FirstOrDefaultAsync(cancellationToken);
            var publisher = mapper.Map<Publisher>(mongoPublisher);
            var sqlPublisher = await sqlContext.Publishers
                .FirstOrDefaultAsync(p => p.Id == publisher.Id, cancellationToken);
            game.Publisher = sqlPublisher ?? publisher;
        }
    }
}