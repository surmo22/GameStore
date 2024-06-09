using GameStore.Data;
using GameStore.Data.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Services.GameService
{
    public class GameService : IGameService
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 8;

        public GameService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Game> CreateGameAsync(Game game, string selectedGenres)
        {
            IList<Genre> genres = new List<Genre>();

            if (!string.IsNullOrEmpty(selectedGenres))
            {
                var genresIds = selectedGenres.Split(',').Select(int.Parse).ToList();
                foreach (var genre in genresIds)
                {
                    var x = await _context.Genres.FindAsync(genre);
                    if (x != null)
                    {
                        genres.Add(x);
                    }
                }
            }
            game.Genres = genres;
            if (game.GameImages != null && game.GameImages.Count == 1)
            {
                game.GameImages = game.GameImages[0].Split(",")
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList();
            }
            await _context.AddAsync(game);
            await _context.SaveChangesAsync();
            return game;
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

        public async Task<Game> EditGameAsync(int id, Game game, string selectedGenres)
        {
            IList<Genre> genres = new List<Genre>();
            if (!string.IsNullOrEmpty(selectedGenres))
            {
                var genresIds = selectedGenres.Split(',').Select(int.Parse).ToList();
                foreach (var genre in genresIds)
                {
                    var x = await _context.Genres.FindAsync(genre);
                    if (x != null)
                    {
                        genres.Add(x);
                    }
                }
            }
            var foundGame = await _context.Games.Include(g => g.Genres).FirstOrDefaultAsync(g => g.Id == id);
            if (foundGame == null)
            {
                return null;
            }
            foundGame.Title = game.Title;
            foundGame.Description = game.Description;
            foundGame.Price = game.Price;
            foundGame.ReleaseDate = game.ReleaseDate;
            foundGame.Publisher = game.Publisher;
            foundGame.Developer = game.Developer;
            foundGame.Platform = game.Platform;
            foundGame.CoverImageUrl = game.CoverImageUrl;
            foundGame.TrailerUrl = game.TrailerUrl;
            foundGame.GameImages = game.GameImages;
            if (foundGame.GameImages != null && foundGame.GameImages.Count == 1)
            {
                foundGame.GameImages = foundGame.GameImages[0].Split(",")
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList();
            }
            if (genres.Count > 0)
            {
                foundGame.Genres?.Clear();
                foreach (var genre in genres)
                {
                    foundGame.Genres?.Add(genre);
                }
            }
            _context.Update(foundGame);
            await _context.SaveChangesAsync();
            return foundGame;
        }
    }
}