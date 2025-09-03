using AutoMapper;
using GameStore.BLL.DTOs.Payment;
using GameStore.BLL.Interfaces.ExternalServices;
using GameStore.BLL.Interfaces.PaymentServices;
using GameStore.BLL.Options;
using GameStore.BLL.Services.ExternalServices.Requests;
using GameStore.BLL.Services.PaymentServices.ResultHandlers;
using GameStore.Domain.Entities.CoreEntities;
using Microsoft.Extensions.Options;

namespace GameStore.BLL.Services.PaymentServices.PaymentHandlers;

public class BoxPaymentHandler(IPaymentExternalService paymentService, IMapper mapper, IOptions<BoxPaymentOptions> options) : IPaymentHandler
{
    private readonly string _methodName = options.Value.Title;

    public async Task<IPaymentResult> PayAsync(
        Order order,
        PaymentRequest paymentRequest,
        CancellationToken cancellationToken)
    {
        var iBoxPaymentRequest = new BoxPaymentRequest
        {
            AccountId = order.CustomerId,
            OrderId = order.Id,
            TransactionAmount = (double)order.TotalSum!,
        };

        var result = await paymentService.PayWithBox(
            options.Value.Endpoint,
            iBoxPaymentRequest);

        var response = mapper.Map<BoxPaymentDto>(result);

        return new IBoxResult(response);
    }

    public bool CanHandle(string paymentHandlerName)
    {
        return paymentHandlerName == _methodName;
    }
}