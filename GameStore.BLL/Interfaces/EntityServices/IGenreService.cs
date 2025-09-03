using GameStore.BLL.DTOs.Games;
using GameStore.BLL.DTOs.Genres;
using GameStore.Common.Exceptions;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Interfaces.EntityServices;

/// <summary>
/// Represents a service for managing genres within the system, including creating, updating,
/// retrieving, and deleting genres, as well as retrieving games associated with specific genres.
/// </summary>
public interface IGenreService
{
    /// <summary>
    /// Asynchronously creates a new genre based on the provided data and returns the created genre.
    /// </summary>
    /// <param name="genreCreateDto">An object containing the details of the genre to be created.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the created genre.</returns>
    Task<GenreDto> CreateGenreAsync(GenreCreateDto genreCreateDto, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a genre by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the genre to be deleted.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteGenreAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all genres asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a collection of <see cref="GenreDto"/> objects representing the genres.</returns>
    Task<IEnumerable<GenreDto>> GetAllGenresAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a genre by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the genre to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="GenreDto"/> corresponding to the specified identifier, or null if no genre is found.</returns>
    Task<GenreDto?> GetGenreByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing genre in the system with the provided details.
    /// </summary>
    /// <param name="genreDto">The genre details to update, including its unique identifier and properties to modify.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <exception cref="InvalidGenreHierarchyException">Thrown if the genre references itself as its parent, creating an invalid hierarchy.</exception>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateGenreAsync(GenreDto genreDto, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a collection of games associated with a specific genre.
    /// </summary>
    /// <param name="id">The unique identifier of the genre.</param>
    /// <param name="cancellationToken">A token that allows the operation to be cancelled.</param>
    /// <returns>A task representing an asynchronous operation containing a collection of <see cref="GameDto"/> objects associated with the genre. Returns null if no games are found.</returns>
    Task<IEnumerable<GameDto>> GetGamesByGenreIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a collection of genres based on the specified list of genre IDs.
    /// </summary>
    /// <param name="genresIds">A list of GUIDs representing the genre IDs to retrieve. Can be null.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a collection of <see cref="Genre"/> objects corresponding to the provided IDs.
    /// </returns>
    Task<ICollection<Genre>> GetGenresByIdsAsync(List<Guid>? genresIds, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all nested genres based on the given genre ID, if they exist.
    /// </summary>
    /// <param name="id">The unique identifier of the parent genre to fetch nested genres for.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>An enumerable collection of <see cref="GenreDto"/> representing the nested genres.</returns>
    Task<IEnumerable<GenreDto>> GetNestedGenresAsync(Guid id, CancellationToken cancellationToken);
}
