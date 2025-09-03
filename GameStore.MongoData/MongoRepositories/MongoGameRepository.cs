using GameStore.BLL.DTOs.Games;
using GameStore.BLL.GamesQueryPipeline.Interfaces;
using GameStore.BLL.Interfaces;
using GameStore.Common.Utils;
using GameStore.Data.Data;
using GameStore.Data.Interfaces;
using GameStore.Domain.MongoEntities;
using GameStore.MongoData.Interfaces.MongoRepositories;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace GameStore.MongoData.MongoRepositories;

public class MongoGameRepository(NorthwindMongoContext context, IMongoLogger logger, IDateTimeProvider timeProvider) : IMongoGameRepository
{
    public async Task<GameList<MongoGame>> GetAllGamesAsync(IGamePipeline gamePipeline,
        CancellationToken cancellationToken)
    {
        var baseQuery = context.Games.AsQueryable();

        var filteredSorted =  (IMongoQueryable<MongoGame>)gamePipeline.ApplyFiltersAndSorting(baseQuery);
        var totalCount = await filteredSorted.CountAsync(cancellationToken);

        var paginated = (IMongoQueryable<MongoGame>)gamePipeline.ApplyPagination(filteredSorted);
        var games = await paginated.ToListAsync(cancellationToken);
        
        return new GameList<MongoGame>
        {
            Games = games,
            Count = totalCount
        };
    }

    public async Task<MongoGame?> GetGameByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var intId = IntToGuidConverter.Convert(id);
        var mongoGame = await context.Games.AsQueryable().Where(p => p.ProductId == intId).FirstOrDefaultAsync(cancellationToken);
        return mongoGame;
    }

    public async Task<MongoGame?> GetGameByKeyAsync(string key, CancellationToken cancellationToken)
    {
        var mongoGame = await context.Games.AsQueryable().Where(p => p.ProductKey == key).FirstOrDefaultAsync(cancellationToken);
        return mongoGame;
    }

    public async Task<bool> GameExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        var intId = IntToGuidConverter.Convert(id);

        var filter = Builders<MongoGame>.Filter.Eq("ProductID", intId);
        var exists = await context.Games.Find(filter).AnyAsync(cancellationToken);

        return exists;
    }

    public async Task UpdateUnitsInStockAsync(Guid id, int delta, CancellationToken cancellationToken)
    {
        var intId = IntToGuidConverter.Convert(id);
        var filter = Builders<MongoGame>.Filter.Eq(g => g.ProductId, intId);
        var update = Builders<MongoGame>.Update.Inc(g => g.UnitsInStock, delta);
        
        var oldDocument = await context.Games.Find(filter).FirstOrDefaultAsync(cancellationToken);
        
        var updatedDocument = await context.Games.FindOneAndUpdateAsync(
            filter,
            update,
            new FindOneAndUpdateOptions<MongoGame>
            {
                ReturnDocument = ReturnDocument.After 
            },
            cancellationToken
        );

        var logs = new List<Log>
        {
            new()
            {
                Action = nameof(UpdateUnitsInStockAsync),
                EntityType = nameof(MongoGame),
                OldVersion = oldDocument is not null ? new { oldDocument.UnitsInStock } : null,
                NewVersion = updatedDocument is not null ? new { updatedDocument.UnitsInStock } : null,
                Timestamp = timeProvider.UtcNow,
            }
        };

        await logger.LogChangesAsync(logs, cancellationToken);
    }

    public async Task IncrementViewCountAsync(Guid id, CancellationToken cancellationToken)
    {
        var intId = IntToGuidConverter.Convert(id);
        var filter = Builders<MongoGame>.Filter.Eq(g => g.ProductId, intId);
        var update = Builders<MongoGame>.Update.Inc(g => g.ViewCount, 1);
        
        var oldDocument = await context.Games.Find(filter).FirstOrDefaultAsync(cancellationToken);
        
        var updatedDocument = await context.Games.FindOneAndUpdateAsync(
            filter,
            update,
            new FindOneAndUpdateOptions<MongoGame>
            {
                ReturnDocument = ReturnDocument.After 
            },
            cancellationToken
        );

        var logs = new List<Log>
        {
            new()
            {
                Action = nameof(IncrementViewCountAsync),
                EntityType = nameof(MongoGame),
                OldVersion = oldDocument is not null ? new { oldDocument.ViewCount } : null,
                NewVersion = updatedDocument is not null ? new { updatedDocument.ViewCount } : null,
                Timestamp = timeProvider.UtcNow,
            }
        };
        
        await logger.LogChangesAsync(logs, cancellationToken);
    }
}