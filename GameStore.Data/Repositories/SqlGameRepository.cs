using GameStore.BLL.DTOs.Games;
using GameStore.BLL.GamesQueryPipeline.Interfaces;
using GameStore.BLL.Interfaces.Repositories;
using GameStore.Data.Data;
using GameStore.Domain.Entities.CoreEntities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Data.Repositories;

public class SqlGameRepository(GameStoreContext context) : IGameRepository
{
    public async Task AddGameAsync(Game game, CancellationToken cancellationToken)
    {
        await context.Games.AddAsync(game, cancellationToken);
    }

    public async Task<GameList<Game>> GetAllGamesAsync(IGamePipeline gamePipeline,
        CancellationToken cancellationToken)
    {
        var query = context.Games.AsNoTracking()
            .AsSplitQuery()
            .Include(g => g.Platforms)
            .Include(g => g.Genres)
            .Include(g => g.Comments)
            .AsQueryable();
        
        var filteredSorted = gamePipeline.ApplyFiltersAndSorting(query);
        var totalCount = await filteredSorted.CountAsync(cancellationToken);
        
        var paginated = gamePipeline.ApplyPagination(filteredSorted);
        
        var games = await paginated.ToListAsync(cancellationToken);
        
        return new GameList<Game>
        {
            Games = games,
            Count = totalCount
        };
    }

    public async Task<Game?> GetGameByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Games
            .AsNoTracking()
            .Include(g => g.Genres)
            .Include(g => g.Platforms)
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<Game?> GetGameByKeyAsync(string key, CancellationToken cancellationToken)
    {
        return await context.Games
            .AsNoTracking()
            .Include(g => g.Genres)
            .Include(g => g.Platforms)
            .Include(g => g.Publisher)
            .FirstOrDefaultAsync(g => g.Key == key, cancellationToken);
    }

    public async Task UpdateGameAsync(Game game, CancellationToken cancellationToken)
    {
        var foundGame = await context.Games
                            .Include(g => g.Genres)
                            .Include(g => g.Platforms)
                            .FirstAsync(g => g.Id == game.Id, cancellationToken);
        context.Entry(foundGame).CurrentValues.SetValues(game);
        foundGame.Genres = game.Genres;
        foundGame.Platforms = game.Platforms;
    }

    public async Task DeleteGameAsync(Guid id, CancellationToken cancellationToken)
    {
        var game = await context.Games.FirstAsync(g => g.Id == id, cancellationToken);
        game.IsDeleted = true;
        context.Games.Update(game);
    }

    public async Task UpdateUnitsInStockAsync(Guid id, int delta, CancellationToken cancellationToken)
    {
        var game = await context.Games.FirstAsync(g => g.Id == id, cancellationToken);
        game.UnitInStock += delta;
        if (game.UnitInStock < 0)
        {
            throw new InvalidOperationException("Not enough units in stock.");
        }
        context.Games.Update(game);
    }

    public async Task IncrementViewCountAsync(Guid id, CancellationToken cancellationToken)
    {
        var game = await context.Games.FirstAsync(g => g.Id == id, cancellationToken);
        game.ViewCount += 1;
        context.Games.Update(game);
    }

    public async Task<bool> GameExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Games.AsNoTracking().AnyAsync(g => g.Id == id, cancellationToken);
    }
}
