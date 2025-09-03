using GameStore.Common.Utils;
using GameStore.Data.Data;
using GameStore.Domain.MongoEntities;
using GameStore.MongoData.Interfaces.MongoRepositories;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace GameStore.MongoData.MongoRepositories;

public class MongoGenreRepository(NorthwindMongoContext context) : IMongoGenreRepository
{
    public async Task<IEnumerable<MongoGenre?>> GetAllGenresAsync(CancellationToken cancellationToken)
    {
        var mongoGenres = await context.Genres.AsQueryable().ToListAsync(cancellationToken);
        return mongoGenres;
    }

    public async Task<MongoGenre?> GetGenreByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var intId = IntToGuidConverter.Convert(id);
        var mongoGenre = await context.Genres.AsQueryable().Where(p => p.CategoryId == intId).FirstOrDefaultAsync(cancellationToken);
        return mongoGenre;
    }

    public async Task<bool> GenreExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        var intId = IntToGuidConverter.Convert(id);

        var exists = await context.Genres.AsQueryable().Where(p => p.CategoryId == intId).AnyAsync(cancellationToken);

        return exists;
    }
}