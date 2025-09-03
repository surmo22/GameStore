using GameStore.Domain.MongoEntities;

namespace GameStore.MongoData.Interfaces.MongoRepositories;

public interface IMongoGenreRepository
{
    Task<IEnumerable<MongoGenre?>> GetAllGenresAsync(CancellationToken cancellationToken);
    Task<MongoGenre?> GetGenreByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> GenreExistsAsync(Guid id, CancellationToken cancellationToken);
}