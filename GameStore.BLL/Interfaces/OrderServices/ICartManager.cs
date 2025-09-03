using GameStore.BLL.DTOs.Games;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Interfaces.OrderServices;

/// <summary>
/// Provides an interface to manage the operations related to a shopping cart within an order.
/// </summary>
public interface ICartManager
{
    /// <summary>
    /// Adds a specified game to an existing order.
    /// </summary>
    /// <param name="order">The order to which the game will be added.</param>
    /// <param name="game">The game to add to the order.</param>
    void AddGameToOrder(Order order, GameDto game);

    /// <summary>
    /// Removes a specified game from an existing order.
    /// </summary>
    /// <param name="order">The order from which the game should be removed.</param>
    /// <param name="game">The game to be removed from the order.</param>
    void RemoveGameFromOrder(Order order, GameDto game);
}