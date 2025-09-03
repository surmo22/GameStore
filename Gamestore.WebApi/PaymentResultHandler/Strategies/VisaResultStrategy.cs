using GameStore.BLL.Interfaces.PaymentServices;
using GameStore.BLL.Services.PaymentServices.ResultHandlers;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.WebApi.PaymentResultHandler.Strategies;

public class VisaResultStrategy : IPaymentResultStrategy
{
    public bool CanHandle(IPaymentResult result) => result is VisaResult;

    public IActionResult Handle(IPaymentResult result)
    {
        return new OkResult();
    }
}