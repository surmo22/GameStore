using GameStore.BLL.Interfaces.PaymentServices;

namespace GameStore.BLL.Services.PaymentServices.ResultHandlers;

public class InvoiceResult(byte[] invoicePdf) : IPaymentResult
{
    public byte[] InvoicePdf { get; } = invoicePdf;
}