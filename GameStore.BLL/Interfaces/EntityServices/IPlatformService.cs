using GameStore.BLL.DTOs.Games;
using GameStore.BLL.DTOs.Platforms;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Interfaces.EntityServices;

/// <summary>
/// Interface for managing platform-related operations within the application.
/// </summary>
public interface IPlatformService
{
    /// <summary>
    /// Creates a new platform asynchronously.
    /// </summary>
    /// <param name="platformCreateDto">An object containing the details of the platform to be created.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <return>A <see cref="PlatformDto"/> object representing the created platform.</return>
    Task<PlatformDto> CreatePlatformAsync(PlatformCreateDto platformCreateDto, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a platform identified by the given ID.
    /// </summary>
    /// <param name="id">The unique identifier of the platform to delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeletePlatformAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all available platforms asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see cref="PlatformDto"/> objects.</returns>
    Task<IEnumerable<PlatformDto>> GetAllPlatformsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a platform by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the platform.</param>
    /// <param name="cancellationToken">A cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="PlatformDto"/> if found, or null if not found.</returns>
    Task<PlatformDto?> GetPlatformByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously retrieves a collection of games associated with the specified platform ID.
    /// </summary>
    /// <param name="id">The unique identifier of the platform.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see cref="GameDto"/> objects associated with the specified platform ID, or null if no games are found.</returns>
    Task<IEnumerable<GameDto>> GetGamesByPlatformIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the information of an existing platform.
    /// </summary>
    /// <param name="platformCreateDto">The data transfer object containing the updated platform information.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdatePlatformAsync(PlatformDto platformCreateDto, CancellationToken cancellationToken);

    /// Asynchronously retrieves a collection of platform entities by their IDs.
    /// <param name="platformIds">A list of platform IDs to retrieve. Can be null.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task that represents the asynchronous operation. The task result contains a collection of platform entities.</return>
    Task<ICollection<Platform>> GetPlatformsByIdsAsync(List<Guid>? platformIds, CancellationToken cancellationToken);
}
