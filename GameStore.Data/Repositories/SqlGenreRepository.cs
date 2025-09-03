using GameStore.BLL.Interfaces.Repositories;
using GameStore.Data.Data;
using GameStore.Domain.Entities.CoreEntities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Data.Repositories;

public class SqlGenreRepository(GameStoreContext context) : IGenreRepository
{
    public async Task<IEnumerable<Genre>> GetAllGenresAsync(CancellationToken cancellationToken)
    {
        return await context.Genres
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Genre?> GetGenreByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Genres
            .Include(g => g.Games)
            .Include(g => g.SubGenres)
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task AddGenreAsync(Genre genre, CancellationToken cancellationToken)
    {
        await context.Genres.AddAsync(genre, cancellationToken);
    }

    public Task UpdateGenre(Genre genre, CancellationToken cancellationToken)
    {
        context.Genres.Update(genre);
        return Task.CompletedTask;
    }

    public async Task DeleteGenreAsync(Guid id, CancellationToken cancellationToken)
    {
        var genre = await GetGenreByIdAsync(id, cancellationToken);
        genre.IsDeleted = true;
        context.Genres.Update(genre);
    }

    public async Task<bool> GenreExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Genres.AsNoTracking().AnyAsync(g => g.Id == id, cancellationToken);
    }
}
