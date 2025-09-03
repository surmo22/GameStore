using GameStore.BLL.Interfaces.PaymentServices;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.WebApi.PaymentResultHandler;

public interface IPaymentResultHandler
{
    IActionResult HandleResult(IPaymentResult result);
}