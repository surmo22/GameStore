using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Interfaces.OrderServices;

/// <summary>
/// Provides functionality to manage orders, including creating or retrieving open orders
/// and deleting empty orders.
/// </summary>
public interface IOrderManager
{
    /// Retrieves the currently open order for the specified customer or creates a new open order if none exists.
    /// <param name="customerId">The unique identifier of the customer.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the customer's open order.</returns>
    Task<Order> GetOrCreateOpenOrderAsync(Guid customerId, CancellationToken cancellationToken);

    /// Deletes the specified order if it contains no items.
    /// <param name="order">The order to be checked and potentially deleted.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteOrderIfEmptyAsync(Order order, CancellationToken cancellationToken);
}