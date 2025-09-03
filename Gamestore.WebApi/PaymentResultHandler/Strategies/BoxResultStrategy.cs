using GameStore.BLL.Interfaces.PaymentServices;
using GameStore.BLL.Services.PaymentServices.ResultHandlers;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.WebApi.PaymentResultHandler.Strategies;

public class BoxResultStrategy : IPaymentResultStrategy
{
    public bool CanHandle(IPaymentResult result) => result is IBoxResult;

    public IActionResult Handle(IPaymentResult result)
    {
        var boxResult = result as IBoxResult;
        return new OkObjectResult(boxResult.Response);
    }
}