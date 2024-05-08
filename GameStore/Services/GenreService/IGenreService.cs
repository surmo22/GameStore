using GameStore.Data;

namespace GameStore.Services.GenreService
{
    public interface IGenreService
    {
        Task<List<Genre>> GetAllGenresAsync();
        Task<Genre?> GetGenreByIdAsync(int id);
        Task CreateGenreAsync(Genre genre);
        Task UpdateGenreAsync(Genre genre);
        Task DeleteGenreAsync(int id);
        Task<ICollection<Game>> GetGamesByGenre(int id);
    }
}
