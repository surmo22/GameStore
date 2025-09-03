using GameStore.BLL.DTOs.Payment;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Interfaces.PaymentServices;

/// <summary>
/// Represents a handler for processing payments in the system.
/// </summary>
public interface IPaymentHandler
{
    /// <summary>
    /// Processes the payment for a given order using the specified payment request details.
    /// </summary>
    /// <param name="order">The order for which the payment is being processed.</param>
    /// <param name="paymentRequest">The payment request containing the payment method and associated data.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation if required.</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the payment process.</returns>
    Task<IPaymentResult> PayAsync(Order order, PaymentRequest paymentRequest, CancellationToken cancellationToken);

    /// <summary>
    /// Determines whether the current payment handler can process the specified payment method.
    /// </summary>
    /// <param name="paymentHandlerName">The name of the payment method to be checked.</param>
    /// <returns>True if the payment handler can process the specified payment method; otherwise, false.</returns>
    bool CanHandle(string paymentHandlerName);
}