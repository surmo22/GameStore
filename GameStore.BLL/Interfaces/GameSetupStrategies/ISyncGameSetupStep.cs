using GameStore.BLL.DTOs;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Interfaces.GameSetupStrategies;

/// <summary>
/// Represents a synchronous step in the game setup process.
/// </summary>
/// <remarks>
/// This interface defines the contract for synchronously initializing
/// specific fields of a game entity during the setup process based
/// on the data provided in the game request.
/// </remarks>
public interface ISyncGameSetupStep : IGameSetupStep
{
    /// <summary>
    /// Initializes a specific field of a game object based on the provided game request data.
    /// </summary>
    /// <param name="game">The game object to update.</param>
    /// <param name="gameRequest">The game request containing data to initialize the game's fields.</param>
    void InitializeField(Game game, IGameRequest gameRequest);
}