using GameStore.BLL.Interfaces.PaymentServices;
using GameStore.Common.Exceptions;

namespace GameStore.BLL.Services.PaymentServices;

public class PaymentHandlerFactory(IEnumerable<IPaymentHandler> paymentHandlers) : IPaymentHandlerFactory
{
    public IPaymentHandler GetPaymentHandler(string paymentMethod)
    {
        var handler = paymentHandlers.FirstOrDefault(x => x.CanHandle(paymentMethod));
        return handler ?? throw new PaymentMethodIsNotSupportedException(paymentMethod);
    }
}