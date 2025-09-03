using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.MongoData.Interfaces;

public interface IDatabaseMigrator
{
    public Task MigrateGameAsync(Game game, Game mongoGame, CancellationToken cancellationToken);
    Task MigratePublisherAsync(Publisher publisher, CancellationToken cancellationToken);
    Task MigrateGenresAsync(IEnumerable<Genre> genres, CancellationToken cancellationToken);
}