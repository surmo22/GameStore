using GameStore.Data;
using GameStore.Data.ViewModels;
using Microsoft.EntityFrameworkCore;

public class GameService : IGameService
{
    private readonly ApplicationDbContext _context;
    private const int PageSize = 8;

    public GameService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GamesViewModel> GetAllGamesAsync(string? searchTerm, int page)
    {
        IQueryable<Game> gamesQuery = _context.Games;

        if (!string.IsNullOrEmpty(searchTerm))
        {
            gamesQuery = gamesQuery.Where(g => g.Title.Contains(searchTerm));
        }

        var totalGames = await gamesQuery.CountAsync();
        var games = await gamesQuery
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        var gamesViewModel = new GamesViewModel
        {
            Games = games,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(totalGames / (double)PageSize)
        };

        return gamesViewModel;
    }

    public async Task<Game> GetGameByIdAsync(int id)
    {
        return await _context.Games.Include(g => g.Genres).FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<GamesViewModel> GetGamesByGenreAsync(int genreId, int page)
    {
        var gamesQuery = _context.Genres.Include(g => g.Games)
            .Where(g => g.Id == genreId)
            .SelectMany(g => g.Games);

        var totalGames = await gamesQuery.CountAsync();
        var games = await gamesQuery
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        var gamesViewModel = new GamesViewModel
        {
            Games = games,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(totalGames / (double)PageSize)
        };

        return gamesViewModel;
    }
}
