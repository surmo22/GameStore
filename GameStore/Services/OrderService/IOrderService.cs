using GameStore.Data;

namespace GameStore.Services.OrderService
{
    public interface IOrderService
    {
        public Task AddOrderAsync(Order order, Data.Cart.CartItem[] cartItems);
    }
}
