using GameStore.Data.Data;
using GameStore.Data.Interfaces;
using GameStore.Domain.MongoEntities;
using MongoDB.Bson;

namespace GameStore.MongoData.Utils;

public class MongoLogger(NorthwindMongoContext context) : IMongoLogger
{
    public async Task LogChangesAsync(List<Log> changes, CancellationToken cancellationToken)
    {
        var logEntries = changes.Select(log => new BsonDocument()
        {
            { "Timestamp", log.Timestamp },
            { "Action", log.Action },
            { "EntityType", log.EntityType },
            { "OldVersion", log.OldVersion is not null ? BsonDocumentWrapper.Create(log.OldVersion) : BsonNull.Value },
            { "NewVersion", log.NewVersion is not null ? BsonDocumentWrapper.Create(log.NewVersion) : BsonNull.Value }
        }).ToList();

        var insertCommand = new BsonDocument()
        {
            { "insert", "EntityChangeLogs" },
            { "documents", new BsonArray(logEntries) },
        };
        
        await context.Database.RunCommandAsync<BsonDocument>(insertCommand, cancellationToken: cancellationToken);
    }
}