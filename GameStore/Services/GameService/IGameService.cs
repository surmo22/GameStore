using GameStore.Data;
using GameStore.Data.ViewModels;

public interface IGameService
{
    Task<GamesViewModel> GetAllGamesAsync(string searchTerm, int page);
    Task<Game> GetGameByIdAsync(int id);
    Task<GamesViewModel> GetGamesByGenreAsync(int genreId, int page);
}
