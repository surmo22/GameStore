using GameStore.Data;
using GameStore.Data.Cart;
using GameStore.Services.OrderService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Controllers
{
    [Route("Checkout"), Authorize]
    public class OrderController : Controller
    {
        private readonly Cart cart;
        private readonly IOrderService orderService;

        public OrderController(Cart cart, IOrderService orderService)
        {
            this.cart = cart;
            this.orderService = orderService;
        }

        [HttpGet]
        public ViewResult Checkout() => this.View(new Order());

        [HttpPost]
        public async Task<IActionResult> CheckoutAsync(Order order)
        {
            if (!this.cart.Lines.Any())
            {
                this.ModelState.AddModelError(key: string.Empty, errorMessage: "Sorry, your cart is empty!");
            }

            if (this.ModelState.IsValid)
            {
                await orderService.AddOrderAsync(order, this.cart.Lines.ToArray());
                this.cart.Clear();
                return this.View(viewName: "Completed", model: order.OrderId);
            }

            return this.View();
        }
    }
}
