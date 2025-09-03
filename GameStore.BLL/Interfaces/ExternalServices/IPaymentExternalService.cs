using GameStore.BLL.Services.ExternalServices.Requests;
using GameStore.BLL.Services.ExternalServices.Responses;

namespace GameStore.BLL.Interfaces.ExternalServices;

/// <summary>
/// Defines methods for interacting with external payment services, providing support for various payment providers.
/// </summary>
public interface IPaymentExternalService
{
    /// <summary>
    /// Initiates a payment process using Visa by sending the payment request to the specified endpoint.
    /// </summary>
    /// <param name="endPoint">The URL endpoint to which the Visa payment request is sent.</param>
    /// <param name="request">The Visa payment request containing the transaction details.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PayWithVisa(string endPoint, VisaPaymentRequest request);

    /// <summary>
    /// Processes a payment using the "PayWithBox" method, sending a request to the specified endpoint
    /// and receiving a response that contains details of the payment transaction.
    /// </summary>
    /// <param name="endPoint">The endpoint URL where the payment request should be sent.</param>
    /// <param name="request">The payment request data, including transaction amount, account information, and order details.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the payment response with details such as user ID, order ID, payment method, account ID, and transaction amount.</returns>
    Task<BoxPaymentResponse> PayWithBox(string endPoint, BoxPaymentRequest request);
}