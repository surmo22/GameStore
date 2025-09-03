using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Interfaces.Repositories;

public interface IGenreRepository
{
    Task<IEnumerable<Genre>> GetAllGenresAsync(CancellationToken cancellationToken);

    Task<Genre?> GetGenreByIdAsync(Guid id, CancellationToken cancellationToken);

    Task AddGenreAsync(Genre genre, CancellationToken cancellationToken);

    Task UpdateGenre(Genre genre, CancellationToken cancellationToken);

    Task DeleteGenreAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> GenreExistsAsync(Guid id, CancellationToken cancellationToken);
}
