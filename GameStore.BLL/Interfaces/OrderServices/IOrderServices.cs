using GameStore.BLL.Interfaces.PaymentServices;

namespace GameStore.BLL.Interfaces.OrderServices;

/// <summary>
/// Provides grouped services for handling orders and shopping cart actions within the GameStore application.
/// </summary>
public interface IOrderServices
{
    /// <summary>
    /// Provides access to operations for managing orders.
    /// </summary>
    /// <remarks>
    /// The <c>OrderManager</c> property grants access to an <c>IOrderManager</c>
    /// implementation, which facilitates creating or retrieving open orders and
    /// deleting empty orders within the system.
    /// </remarks>
    IOrderManager OrderManager { get; }

    /// <summary>
    /// Provides access to the functionalities for managing the shopping cart within an order system.
    /// </summary>
    ICartManager CartManager { get; }
    
    IPaymentService PaymentService { get; }
}