using GameStore.BLL.Interfaces.PaymentServices;
using GameStore.WebApi.PaymentResultHandler.Strategies;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.WebApi.PaymentResultHandler;

public class PaymentResultHandler(IEnumerable<IPaymentResultStrategy> strategies) : IPaymentResultHandler
{
    public IActionResult HandleResult(IPaymentResult result)
    {
        var strategy = strategies.FirstOrDefault(s => s.CanHandle(result));
        return strategy is not null ? strategy.Handle(result) : new StatusCodeResult(500);
    }
}