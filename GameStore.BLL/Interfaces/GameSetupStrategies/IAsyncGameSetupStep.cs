using GameStore.BLL.DTOs;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Interfaces.GameSetupStrategies;

/// <summary>
/// Defines an asynchronous game setup step which is responsible for initializing specific fields
/// of a <see cref="Game"/> entity based on the provided <see cref="IGameRequest"/> during the game
/// setup process.
/// </summary>
public interface IAsyncGameSetupStep : IGameSetupStep
{
    /// <summary>
    /// Asynchronously initializes a specific field of the provided game entity based on the game request data.
    /// </summary>
    /// <param name="game">The game entity whose field needs initialization.</param>
    /// <param name="gameRequest">The data source containing the information required for field initialization.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InitializeFieldAsync(Game game, IGameRequest gameRequest, CancellationToken cancellationToken);
}