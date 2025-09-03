using GameStore.BLL.DTOs.Payment;
using GameStore.BLL.Interfaces.PaymentServices;

namespace GameStore.BLL.Services.PaymentServices.ResultHandlers;

public class IBoxResult(BoxPaymentDto response) : IPaymentResult
{
    public BoxPaymentDto Response { get; } = response;
}