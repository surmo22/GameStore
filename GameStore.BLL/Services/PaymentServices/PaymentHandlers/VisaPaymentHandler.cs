using GameStore.BLL.DTOs.Payment;
using GameStore.BLL.Interfaces.ExternalServices;
using GameStore.BLL.Interfaces.PaymentServices;
using GameStore.BLL.Options;
using GameStore.BLL.Services.ExternalServices.Requests;
using GameStore.Domain.Entities.CoreEntities;
using Microsoft.Extensions.Options;

namespace GameStore.BLL.Services.PaymentServices.PaymentHandlers;

public class VisaPaymentHandler(IPaymentExternalService paymentExternalService, IOptions<VisaPaymentOptions> options) : IPaymentHandler
{
    private readonly string _methodName = options.Value.Title;

    public async Task<IPaymentResult> PayAsync(Order order, PaymentRequest paymentRequest, CancellationToken cancellationToken)
    {
        var visaPaymentRequest = new VisaPaymentRequest
        {
            CardNumber = paymentRequest.Visa.CardNumber,
            CardHolderName = paymentRequest.Visa.Holder,
            Cvv = paymentRequest.Visa.Cvv2,
            ExpirationMonth = paymentRequest.Visa.MonthExpire,
            ExpirationYear = paymentRequest.Visa.YearExpire,
            TransactionAmount = (double)order.TotalSum!,
        };

        await paymentExternalService.PayWithVisa(
            options.Value.Endpoint,
            visaPaymentRequest);
        
        return null;
    }

    public bool CanHandle(string paymentHandlerName)
    {
        return paymentHandlerName == _methodName;
    }
}