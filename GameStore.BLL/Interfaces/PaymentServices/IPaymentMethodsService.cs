using GameStore.BLL.DTOs.PaymentMethods;

namespace GameStore.BLL.Interfaces.PaymentServices;

public interface IPaymentMethodsService
{
    Task<PaymentMethodList> GetAllPaymentMethodsAsync(CancellationToken cancellationToken);
}