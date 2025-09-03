using AutoMapper;
using GameStore.BLL.DTOs.Games;
using GameStore.BLL.DTOs.Genres;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.BLL.Interfaces.Repositories;
using GameStore.Common.Exceptions;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Services.EntityServices;

public class GenreService(IGuidProvider guidProvider, IUnitOfWork unitOfWork, IMapper mapper) : IGenreService
{
    public async Task<GenreDto> CreateGenreAsync(GenreCreateDto genreCreateDto, CancellationToken cancellationToken)
    {
        var genre = mapper.Map<Genre>(genreCreateDto);
        genre.Id = guidProvider.NewGuid();
        await unitOfWork.Genres.AddGenreAsync(genre, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return mapper.Map<GenreDto>(genre);
    }

    public async Task DeleteGenreAsync(Guid id, CancellationToken cancellationToken)
    {
        await EnsureGenreExistAsync(id, cancellationToken);

        await unitOfWork.Genres.DeleteGenreAsync(id, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
    }

    public async Task<IEnumerable<GenreDto>> GetAllGenresAsync(CancellationToken cancellationToken)
    {
        var genres = await unitOfWork.Genres.GetAllGenresAsync(cancellationToken);
        return mapper.Map<IEnumerable<GenreDto>>(genres);
    }

    public async Task<GenreDto?> GetGenreByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var genre = await unitOfWork.Genres.GetGenreByIdAsync(id, cancellationToken);
        return mapper.Map<GenreDto>(genre);
    }

    public async Task UpdateGenreAsync(GenreDto genreDto, CancellationToken cancellationToken)
    {
        var genre = mapper.Map<Genre>(genreDto);
        await EnsureGenreExistAsync(genre.Id, cancellationToken);

        if (genre.Id == genre.ParentGenreId)
        {
            throw new InvalidGenreHierarchyException();
        }

        await unitOfWork.Genres.UpdateGenre(genre, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
    }

    public async Task<IEnumerable<GameDto>> GetGamesByGenreIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var genre = await unitOfWork.Genres.GetGenreByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException($"No Genre was found with Id {id}");
        var games = genre.Games;
        return mapper.Map<IEnumerable<GameDto>>(games);
    }

    public async Task<ICollection<Genre>> GetGenresByIdsAsync(List<Guid>? genresIds, CancellationToken cancellationToken)
    {
        var genres = new List<Genre>();
        foreach (var id in genresIds)
        {
            var genre = await unitOfWork.Genres.GetGenreByIdAsync(id, cancellationToken)
                ?? throw new KeyNotFoundException($"Genre with key {id} was not found");
            genres.Add(genre);
        }

        return genres;
    }

    public async Task<IEnumerable<GenreDto>> GetNestedGenresAsync(Guid id, CancellationToken cancellationToken)
    {
        await EnsureGenreExistAsync(id, cancellationToken);
        var genre = await unitOfWork.Genres.GetGenreByIdAsync(id, cancellationToken);
        return mapper.Map<IEnumerable<GenreDto>>(genre.SubGenres);
    }

    private async Task EnsureGenreExistAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!await unitOfWork.Genres.GenreExistsAsync(id, cancellationToken))
        {
            throw new KeyNotFoundException($"No Genre was found with ID {id}");
        }
    }
}
