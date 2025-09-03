using GameStore.BLL.Interfaces.PaymentServices;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.WebApi.PaymentResultHandler.Strategies;

public interface IPaymentResultStrategy
{
    bool CanHandle(IPaymentResult result);

    IActionResult Handle(IPaymentResult result);
}