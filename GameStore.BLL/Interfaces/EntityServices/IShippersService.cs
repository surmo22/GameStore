using GameStore.BLL.DTOs;

namespace GameStore.BLL.Interfaces.EntityServices;

/// <summary>
/// Provides methods for managing and retrieving shippers' information.
/// </summary>
public interface IShippersService
{
    /// Asynchronously retrieves all shippers from the repository and maps them to data transfer objects.
    /// <param name="cancellationToken">
    /// A token that can be used by other objects or threads to receive notice of cancellation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a collection of <see cref="GetShipperDto"/>.
    /// </returns>
    Task<IEnumerable<GetShipperDto>> GetAllShippersAsync(CancellationToken cancellationToken);
}