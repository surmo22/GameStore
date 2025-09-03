using System.Security.Claims;
using GameStore.BLL.DTOs.Orders;
using GameStore.BLL.DTOs.Payment;
using GameStore.BLL.Interfaces.OrderServices;
using GameStore.BLL.Interfaces.PaymentServices;
using GameStore.Domain.Entities.Enums;
using GameStore.WebApi.PaymentResultHandler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.WebApi.Controllers;

[Route("[controller]")]
[Authorize]
public class OrdersController(
    IOrderService orderService,
    IPaymentMethodsService paymentMethodsService,
    IPaymentResultHandler resultHandler) : ControllerBase
{
    /// <summary>
    /// Gets a list of paid and canceled orders.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of paid and canceled orders.</returns>
    /// <response code="200">Returns the list of orders.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaidAndCancelledOrders(CancellationToken cancellationToken)
    {
        return Ok(await orderService.GetPaidAndCancelledOrdersAsync(cancellationToken));
    }

    /// <summary>
    /// Retrieves the order history within the specified date range.
    /// </summary>
    /// <param name="startDate">The start date of the order history filter. If null, defaults to the earliest possible date.</param>
    /// <param name="endDate">The end date of the order history filter. If null, defaults to the latest possible date.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>An action result containing the filtered order history.</returns>
    [HttpGet("history")]
    [Authorize(Policy = nameof(UserPermissionTypes.ViewOrderHistory))]
    public async Task<IActionResult> GetHistory(DateTime? startDate, DateTime? endDate,
        CancellationToken cancellationToken)
    {
        return Ok(await orderService
            .GetOrderHistory(startDate ?? DateTime.MinValue, endDate ?? DateTime.MaxValue, cancellationToken));
    }

    /// <summary>
    /// Gets an order by its ID.
    /// </summary>
    /// <param name="id">The order ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The order details.</returns>
    /// <response code="200">Returns the order details.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrderById(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await orderService.GetOrderByIdAsync(id, cancellationToken));
    }

    /// <summary>
    /// Gets the order details for an order with the specified ID.
    /// </summary>
    /// <param name="id">The order ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The order details.</returns>
    /// <response code="200">Returns the order details.</response>
    /// <response code="404">If the order is not found.</response>
    [HttpGet("{id:guid}/details")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderDetails(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await orderService.GetOrderDetailsByIdAsync(id, cancellationToken));
    }

    /// <summary>
    /// Removes a game from the cart.
    /// </summary>
    /// <param name="key">The cart item key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="204">The game was removed from the cart.</response>
    /// <response code="404">If the game is not found or the cart is not found.</response>
    [HttpDelete("cart/{key}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveGameFromCart(string key, CancellationToken cancellationToken)
    {
        var user = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("Please log in"));
        await orderService.RemoveGameFromCartAsync(user, key, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Gets the current user's cart.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The user's cart.</returns>
    /// <response code="200">Returns the user's cart.</response>
    [HttpGet("cart")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCart(CancellationToken cancellationToken)
    {
        var user = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("Please log in"));
        var order = await orderService.GetUserOpenOrderAsync(user, cancellationToken);
        return Ok(order);
    }

    /// <summary>
    /// Gets a list of available payment methods.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of payment methods.</returns>
    /// <response code="200">Returns the list of payment methods.</response>
    [HttpGet("payment-methods")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaymentMethods(CancellationToken cancellationToken)
    {
        return Ok(await paymentMethodsService.GetAllPaymentMethodsAsync(cancellationToken));
    }

    /// <summary>
    /// Updates the quantity of an order detail.
    /// </summary>
    /// <param name="id">The unique identifier of the order detail to update.</param>
    /// <param name="request">The new quantity value to set.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An IActionResult indicating the result of the update operation.</returns>
    [HttpPatch("details/{id:guid}/quantity")]
    [Authorize(Policy = nameof(UserPermissionTypes.EditOrders))]
    public async Task<IActionResult> UpdateQuantity(Guid id, [FromBody] UpdateQuantityRequest request, CancellationToken cancellationToken)
    {
        await orderService.UpdateOrderDetailsQuantity(id, request.Count, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Deletes an order item by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the order item to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An IActionResult indicating the result of the operation.</returns>
    [HttpDelete("details/{id:guid}")]
    [Authorize(Policy = nameof(UserPermissionTypes.EditOrders))]
    public async Task<IActionResult> DeleteOrderItem(Guid id, CancellationToken cancellationToken)
    {
        await orderService.DeleteOrderItem(id, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Adds a game to an existing order.
    /// </summary>
    /// <param name="id">The unique identifier of the order to which the game will be added.</param>
    /// <param name="key">The unique key of the game to add.</param>
    /// <param name="cancellationToken">The cancellation token to terminate the operation if needed.</param>
    /// <returns>An action result indicating the outcome of the operation.</returns>
    [HttpPost("{id:guid}/details/{key}")]
    [Authorize(Policy = nameof(UserPermissionTypes.EditOrders))]
    public async Task<IActionResult> AddGameToOrder(Guid id, string key, CancellationToken cancellationToken)
    {
        await orderService.AddGameToOrder(id, key, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Sets order as shipped to customer.
    /// </summary>
    /// <param name="id">Order id.</param>
    /// <param name="cancellationToken">CancellationToken.</param>
    /// <returns></returns>
    [HttpPost("{id:guid}/ship")]
    [Authorize(Policy = nameof(UserPermissionTypes.ChangeOrderStatus))]
    public async Task<IActionResult> ShipOrder(Guid id, CancellationToken cancellationToken)
    {
        await orderService.ShipOrderAsync(id, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Processes payment for an order.
    /// </summary>
    /// <param name="paymentRequest">The payment request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the payment operation.</returns>
    /// <response code="200">Payment was successful.</response>
    /// <response code="400">If the payment request is invalid.</response>
    /// <response code="404">If the order is not found.</response>
    [HttpPost("payment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PayForOrder([FromBody] PaymentRequest paymentRequest, CancellationToken cancellationToken)
    {
        var user = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("Please log in"));
        var result = await orderService.PayForOrderAsync(user, paymentRequest, cancellationToken);

        return resultHandler.HandleResult(result);
    }
}
