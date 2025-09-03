using GameStore.BLL.DTOs;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Interfaces.GameServices;

/// <summary>
/// Defines the operations required for setting up and building a game within the system.
/// </summary>
public interface IGameSetupService
{
    /// Asynchronously builds a game entity based on the provided game and game request details.
    /// This method performs various setup operations required to configure a game before it is saved or processed further.
    /// <param name="game">The game entity to be built or modified during the setup process.</param>
    /// <param name="gameRequest">The request object containing details and parameters required for the game setup.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task BuildGameAsync(Game game, IGameRequest gameRequest, CancellationToken cancellationToken);
}