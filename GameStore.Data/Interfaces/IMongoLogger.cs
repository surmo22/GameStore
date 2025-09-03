using GameStore.Domain.MongoEntities;

namespace GameStore.Data.Interfaces;

public interface IMongoLogger
{
    Task LogChangesAsync(List<Log> changes, CancellationToken cancellationToken);
}