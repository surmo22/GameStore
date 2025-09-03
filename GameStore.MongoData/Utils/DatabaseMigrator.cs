using GameStore.BLL.Interfaces;
using GameStore.Data.Data;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.MongoData.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameStore.MongoData.Utils;

public class DatabaseMigrator(IGuidProvider guidProvider, GameStoreContext context, ILogger<DatabaseMigrator> logger) : IDatabaseMigrator
{
    public async Task MigrateGameAsync(Game game, Game mongoGame, CancellationToken cancellationToken)
    {
        if (await context.Games.AnyAsync(g => g.Key == game.Key, cancellationToken))
        {
            return;
        }
        
        await MigrateGenresAsync(game.Genres, cancellationToken);
        await MigratePublisherAsync(game.Publisher, cancellationToken);
        
        await context.Games.AddAsync(game, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        await AddAnotherEntityIfKeyChanged(game, mongoGame, cancellationToken);
    }


    public async Task MigratePublisherAsync(Publisher? publisher, CancellationToken cancellationToken)
    {
        if (publisher is not null)
        {
            var publisherExists = await context.Publishers.AnyAsync(p => p.Id == publisher.Id, cancellationToken);
            if (!publisherExists)
            {
                try
                {
                    await context.Publishers.AddAsync(publisher, cancellationToken);
                    await context.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to migrate publisher");
                }
            }
        }
    }

    public async Task MigrateGenresAsync(IEnumerable<Genre> genres, CancellationToken cancellationToken)
    {
        var existingGenreIds = await context.Genres
            .Where(g => genres.Select(genre => genre.Id).Contains(g.Id))
            .Select(g => g.Id)
            .ToListAsync(cancellationToken);
        
        var newGenres = genres.Where(genre => !existingGenreIds.Contains(genre.Id)).ToList();

        if (newGenres.Count > 0)
        {
            await context.Genres.AddRangeAsync(newGenres, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
    
    private async Task AddAnotherEntityIfKeyChanged(Game game, Game mongoGame, CancellationToken cancellationToken)
    {
        if (mongoGame.Key != game.Key && !await context.Games.AnyAsync(g => g.Key == mongoGame.Key || g.Id == game.Id, cancellationToken))
        {
            mongoGame.IsDeleted = true;
            mongoGame.Id = guidProvider.NewGuid();
            await context.Games.AddAsync(mongoGame, cancellationToken);
            await context.SaveChangesAsync(cancellationToken); 
        }
    }
}
