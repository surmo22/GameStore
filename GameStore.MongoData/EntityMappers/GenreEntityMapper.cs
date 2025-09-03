using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.MongoData.EntityMappers;

public static class GenreEntityMapper
{
    public static IEnumerable<Genre> MapGenresById(IEnumerable<Genre> sqlGenres, IEnumerable<Genre> mongoGenres)
    {
        var genres = new List<Genre>(sqlGenres);

        foreach (var mongoGenre in mongoGenres)
        {
            var exists = genres.Any(g => g.Id == mongoGenre.Id);
            if (!exists)
            {
                genres.Add(mongoGenre);
            }
        }

        return genres.Where(g => !g.IsDeleted);
    }
}