using GameStore.BLL.Interfaces.OrderServices;
using GameStore.BLL.Interfaces.PaymentServices;

namespace GameStore.BLL.Services.OrderServices;

public class OrderServices(
    IOrderManager   orderManager,
    ICartManager    cartManager,
    IPaymentService paymentService) : IOrderServices
{
    public IOrderManager OrderManager => orderManager;

    public ICartManager CartManager => cartManager;
    
    public IPaymentService PaymentService => paymentService;
}