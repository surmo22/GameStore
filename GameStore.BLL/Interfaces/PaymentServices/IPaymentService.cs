using GameStore.BLL.DTOs.Payment;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Interfaces.PaymentServices;

public interface IPaymentService
{
    Task<IPaymentResult> PayForOrderAsync(Order order, PaymentRequest paymentRequest, CancellationToken cancellationToken);
}