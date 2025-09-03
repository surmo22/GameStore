using GameStore.BLL.DTOs.Payment;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Interfaces.PaymentServices;
using GameStore.BLL.Options;
using GameStore.BLL.Services.PaymentServices.ResultHandlers;
using GameStore.Domain.Entities.CoreEntities;
using Microsoft.Extensions.Options;

namespace GameStore.BLL.Services.PaymentServices.PaymentHandlers;

public class BankPaymentHandler(IPdfCreator pdfCreator, IOptions<BankOptions> options) : IPaymentHandler
{
    private readonly string _methodName = options.Value.Title;

    public async Task<IPaymentResult> PayAsync(
        Order order,
        PaymentRequest paymentRequest,
        CancellationToken cancellationToken)
    {
        var invoicePdf = await Task.Run(() => pdfCreator.GenerateInvoicePdf(order, options.Value.InvoiceDaysValid), cancellationToken);

        return new InvoiceResult(invoicePdf);
    }

    public bool CanHandle(string paymentHandlerName)
    {
        return paymentHandlerName == _methodName;
    }
}