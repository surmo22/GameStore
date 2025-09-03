using GameStore.BLL.DTOs.Orders;
using GameStore.BLL.DTOs.Payment;
using GameStore.BLL.Interfaces.PaymentServices;

namespace GameStore.BLL.Interfaces.OrderServices;

/// <summary>
/// Represents a service for managing and processing orders in the system.
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Adds a game to the user's shopping cart asynchronously.
    /// </summary>
    /// <param name="userId">The unique identifier of the user adding the game to their cart.</param>
    /// <param name="gameKey">The unique key representing the game to be added to the cart.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddGameToCartAsync(Guid userId, string gameKey, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a game from the user's cart asynchronously.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose cart is being modified.</param>
    /// <param name="gameKey">The unique key of the game to be removed from the cart.</param>
    /// <param name="cancellationToken">A token that propagates notification that operations should be canceled.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveGameFromCartAsync(Guid userId, string gameKey, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a list of orders that are either paid or cancelled.
    /// </summary>
    /// <param name="cancellationToken">A token to signal the cancellation of the operation.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains a list of <see cref="OrderDto"/> objects representing the paid and cancelled orders.</returns>
    Task<IList<OrderDto>> GetPaidAndCancelledOrdersAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves an order by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the order to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="OrderDto"/> representing the order.</returns>
    Task<OrderDto> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the details of an order by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the order.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A list of <see cref="OrderGameDto"/> representing the details of the specified order.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if no order with the specified ID is found.</exception>
    Task<IList<OrderGameDto>> GetOrderDetailsByIdAsync(Guid id, CancellationToken cancellationToken);

    /// Retrieves the open order for a specific user, if it exists.
    /// <param name="user">
    /// The unique identifier of the user whose open order is being retrieved.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to observe for cancellation requests.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a list of order details as <see cref="OrderGameDto"/> objects,
    /// or an empty list if no open order exists for the user.
    /// </returns>
    Task<IList<OrderGameDto>> GetUserOpenOrderAsync(Guid user, CancellationToken cancellationToken);

    /// <summary>
    /// Processes payment for a user's open order using the specified payment details.
    /// </summary>
    /// <param name="user">The unique identifier of the user making the payment.</param>
    /// <param name="paymentRequest">The payment details required to process the order.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the payment result.</returns>
    Task<IPaymentResult> PayForOrderAsync(Guid user, PaymentRequest paymentRequest, CancellationToken cancellationToken);

    /// <summary>
    /// Ships an order by updating its status to "Shipped" if it is currently paid and not already shipped.
    /// </summary>
    /// <param name="id">The unique identifier of the order to be shipped.</param>
    /// <param name="cancellationToken">A token used to propagate notification that the operation should be canceled.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when no order is found with the provided identifier.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the order is not in a paid status.</exception>
    Task ShipOrderAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an order item identified by the specified ID. If the order contains no more items
    /// after the deletion, the order status is updated to cancelled.
    /// </summary>
    /// <param name="id">The unique identifier of the order item to be deleted.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteOrderItem(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the quantity of a specific item within an order.
    /// </summary>
    /// <param name="id">The unique identifier of the order item to update.</param>
    /// <param name="count">The new quantity for the specified order item.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateOrderDetailsQuantity(Guid id, int count, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a game to an existing order identified by the specified order ID.
    /// </summary>
    /// <param name="id">The unique identifier of the order to which the game will be added.</param>
    /// <param name="key">The unique key identifying the game to be added to the order.</param>
    /// <param name="cancellationToken">A token for canceling the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddGameToOrder(Guid id, string key, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the order history within a specified date range.
    /// </summary>
    /// <param name="startDate">The start date of the order history range.</param>
    /// <param name="endDate">The end date of the order history range.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A list of <see cref="OrderDto"/> objects representing the orders within the specified date range.</returns>
    Task<IList<OrderDto>> GetOrderHistory(DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
}