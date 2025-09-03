using GameStore.Common.Exceptions;

namespace GameStore.BLL.Interfaces.PaymentServices;

/// <summary>
/// Defines a factory for retrieving instances of payment handlers based on the payment method.
/// </summary>
public interface IPaymentHandlerFactory
{
    /// <summary>
    /// Retrieves a payment handler capable of processing a specific payment method.
    /// </summary>
    /// <param name="paymentMethod">The name or identifier of the payment method for which a handler is required.</param>
    /// <returns>An instance of <see cref="IPaymentHandler"/> capable of processing the specified payment method.</returns>
    /// <exception cref="PaymentMethodIsNotSupportedException">
    /// Thrown if no payment handler can handle the specified payment method.
    /// </exception>
    public IPaymentHandler GetPaymentHandler(string paymentMethod);
}