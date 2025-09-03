using AutoMapper;
using GameStore.BLL.DTOs.PaymentMethods;
using GameStore.BLL.Interfaces.PaymentServices;
using GameStore.BLL.Interfaces.Repositories;

namespace GameStore.BLL.Services.PaymentServices;

public class PaymentMethodsService(IPaymentMethodsRepository repository, IMapper mapper) : IPaymentMethodsService
{
    public async Task<PaymentMethodList> GetAllPaymentMethodsAsync(CancellationToken cancellationToken)
    {
        var paymentMethods = await repository.GetAllPaymentMethodsAsync(cancellationToken);
        var paymentMethodsList = new PaymentMethodList()
        {
            PaymentMethods = paymentMethods.Select(mapper.Map<PaymentMethodDto>).ToList(),
        };

        return paymentMethodsList;
    }
}