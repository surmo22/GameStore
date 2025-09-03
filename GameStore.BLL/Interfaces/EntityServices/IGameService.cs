using GameStore.BLL.DTOs.Games;
using GameStore.BLL.DTOs.Genres;
using GameStore.BLL.DTOs.Platforms;
using GameStore.BLL.DTOs.Publisher;

namespace GameStore.BLL.Interfaces.EntityServices;

/// <summary>
/// Provides methods for managing games within the Game Store.
/// </summary>
public interface IGameService
{
    /// Asynchronously creates a new game based on the provided game creation details.
    /// <param name="gameCreateDto">
    /// The details for creating a new game, encapsulated within a <see cref="GameCreateRequestDto"/> object.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <return>
    /// A task that represents the asynchronous operation. The task result contains the created game details encapsulated in a <see cref="GameDto"/> object.
    /// </return>
    Task<GameDto> CreateGameAsync(GameCreateRequestDto gameCreateDto, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing game with the provided details.
    /// </summary>
    /// <param name="gameUpdateDto">The DTO containing the updated game information.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task contains the updated game details as a <see cref="GameDto"/>.</returns>
    Task<GameDto> UpdateGameAsync(GameUpdateRequestDto gameUpdateDto, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a game identified by its unique key.
    /// </summary>
    /// <param name="key">The unique key of the game to be deleted.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteGameAsync(string key, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a game by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the game.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="GameDto"/> if found, or null if no game is associated with the provided ID.</returns>
    Task<GameDto?> GetGameByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a paginated list of games based on the specified filter criteria.
    /// </summary>
    /// <param name="filterRequest">The filter criteria used to retrieve the games.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation that contains a paginated list of <see cref="GameDto"/>.</returns>
    Task<PaginatedList<GameDto>> GetAllGamesAsync(GameFilterRequest filterRequest, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a collection of genres associated with a specific game identified by its key.
    /// </summary>
    /// <param name="gameKey">The unique key of the game.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, containing a collection of <see cref="GenreDto"/> objects.</returns>
    Task<IEnumerable<GenreDto>> GetGenresByGameKeyAsync(string gameKey, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a collection of platform details associated with a specific game key.
    /// </summary>
    /// <param name="gameKey">The unique identifier of the game.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see cref="PlatformDto"/> objects.</returns>
    Task<IEnumerable<PlatformDto>> GetPlatformsByGameKeyAsync(string gameKey, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a game by its unique key asynchronously.
    /// </summary>
    /// <param name="gameKey">The unique key of the game to retrieve.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="GameDto"/> representing the game if found; otherwise, null.</returns>
    Task<GameDto?> GetGameByKeyAsync(string gameKey, CancellationToken cancellationToken);

    /// Retrieves the publisher associated with a specific game key.
    /// <param name="gameKey">The unique key of the game for which the publisher is to be retrieved.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A <see cref="PublisherDto"/> containing information about the publisher associated with the specified game key.</return>
    Task<PublisherDto> GetPublisherByGameKeyAsync(string gameKey, CancellationToken cancellationToken);

    /// Updates the stock quantity of a game by applying a delta value.
    /// <param name="id">The unique identifier of the game to update.</param>
    /// <param name="delta">The change in stock levels. Positive values increase stock, negative values decrease it.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation if needed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateGameStockAsync(Guid id, int delta, CancellationToken cancellationToken);

    /// Retrieves a list of all games without applying any filters.
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <return>
    /// A task that represents the asynchronous operation. The task result contains a list of games as <see cref="GameDto"/> objects.
    /// </return>
    Task<IList<GameDto>> GetAllGamesWithoutFilters(CancellationToken cancellationToken);

    /// Retrieves the image associated with a game using its unique key.
    /// <param name="key">The unique key of the game to retrieve the image for.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A tuple containing the image content as a byte array and its content type as a string.</returns>
    Task<(byte[] Content, string ContentType)> GetImageByGameKeyAsync(string key,
        CancellationToken cancellationToken);
}
