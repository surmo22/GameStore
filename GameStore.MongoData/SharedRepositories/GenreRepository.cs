using AutoMapper;
using GameStore.BLL.Interfaces.Repositories;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.MongoData.EntityMappers;
using GameStore.MongoData.Interfaces.MongoRepositories;

namespace GameStore.MongoData.SharedRepositories;

public class GenreRepository(IGenreRepository genreRepository, IMongoGenreRepository mongoGenreRepository, IMapper mapper) : IGenreRepository
{
    public async Task<IEnumerable<Genre>> GetAllGenresAsync(CancellationToken cancellationToken)
    {
        var sqlGenresTask = genreRepository.GetAllGenresAsync(cancellationToken);
        var mongoGenresTask = mongoGenreRepository.GetAllGenresAsync(cancellationToken);
        
        await Task.WhenAll(sqlGenresTask, mongoGenresTask);
        
        var sqlGenres = await sqlGenresTask;
        var mongoGenres = await mongoGenresTask;
        var mappedMongoGenres = mapper.Map<IEnumerable<Genre>>(mongoGenres);
        
        return GenreEntityMapper.MapGenresById(sqlGenres, mappedMongoGenres);
    }

    public async Task<Genre?> GetGenreByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var sqlGenre = await genreRepository.GetGenreByIdAsync(id, cancellationToken) ?? mapper.Map<Genre>(await mongoGenreRepository.GetGenreByIdAsync(id, cancellationToken));

        return sqlGenre;
    }

    public async Task AddGenreAsync(Genre genre, CancellationToken cancellationToken)
    {
        await genreRepository.AddGenreAsync(genre, cancellationToken);
    }

    public async Task UpdateGenre(Genre genre, CancellationToken cancellationToken)
    {
        if (await genreRepository.GenreExistsAsync(genre.Id, cancellationToken))
        {
            await genreRepository.UpdateGenre(genre, cancellationToken);
            return;
        }
        
        await genreRepository.AddGenreAsync(genre, cancellationToken);
    }

    public async Task DeleteGenreAsync(Guid id, CancellationToken cancellationToken)
    {
        if (await genreRepository.GenreExistsAsync(id, cancellationToken))
        {
            await genreRepository.DeleteGenreAsync(id, cancellationToken);
            return;
        }
        
        var mongoGenre = await mongoGenreRepository.GetGenreByIdAsync(id, cancellationToken);
        var mappedGenre = mapper.Map<Genre>(mongoGenre);
        mappedGenre.IsDeleted = true;
        await genreRepository.AddGenreAsync(mappedGenre, cancellationToken);
    }

    public async Task<bool> GenreExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await genreRepository.GenreExistsAsync(id, cancellationToken) ||
               await mongoGenreRepository.GenreExistsAsync(id, cancellationToken);
    }
}