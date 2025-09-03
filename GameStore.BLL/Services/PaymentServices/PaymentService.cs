using GameStore.BLL.DTOs.Payment;
using GameStore.BLL.Interfaces.PaymentServices;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Services.PaymentServices;

public class PaymentService(IPaymentHandlerFactory paymentHandlerFactory) : IPaymentService
{
    public async Task<IPaymentResult> PayForOrderAsync(Order order, PaymentRequest paymentRequest, CancellationToken cancellationToken)
    {
        var paymentHandler = paymentHandlerFactory.GetPaymentHandler(paymentRequest.Method);
        var result = await paymentHandler.PayAsync(order, paymentRequest, cancellationToken);
        return result;
    }
}