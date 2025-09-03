using GameStore.BLL.Interfaces.PaymentServices;
using GameStore.BLL.Services.PaymentServices.ResultHandlers;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.WebApi.PaymentResultHandler.Strategies;

public class InvoiceResultStrategy : IPaymentResultStrategy
{
    public bool CanHandle(IPaymentResult result) => result is InvoiceResult;

    public IActionResult Handle(IPaymentResult result)
    {
        var invoiceResult = result as InvoiceResult;
        return new FileContentResult(invoiceResult.InvoicePdf, "application/pdf");
    }
}