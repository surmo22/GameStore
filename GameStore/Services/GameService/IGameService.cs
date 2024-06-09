using GameStore.Data;
using GameStore.Data.ViewModels;

public interface IGameService
{
    Task<Game> CreateGameAsync(Game game, string selectedGenres);
    Task<Game> EditGameAsync(int id, Game game, string selectedGenres);
    Task<GamesViewModel> GetAllGamesAsync(string? searchTerm, int page);
    Task<Game> GetGameByIdAsync(int id);
    Task<GamesViewModel> GetGamesByGenreAsync(int genreId, int page);
}
