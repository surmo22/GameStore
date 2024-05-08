using GameStore.Data;
using GameStore.Data.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Controllers
{
    [Route("Checkout"), Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext context;

        private readonly Cart cart;

        public OrderController(ApplicationDbContext orderRepository, Cart cart)
        {
            this.context = orderRepository;
            this.cart = cart;
        }

        [HttpGet]
        public ViewResult Checkout() => this.View(new Order());

        [HttpPost]
        public IActionResult Checkout(Order order)
        {
            if (!this.cart.Lines.Any())
            {
                this.ModelState.AddModelError(key: string.Empty, errorMessage: "Sorry, your cart is empty!");
            }

            if (this.ModelState.IsValid)
            {
                order.Lines = this.cart.Lines.ToArray();
                //TODO: save to db
                this.cart.Clear();
                return this.View(viewName: "Completed", model: order.OrderId);
            }

            return this.View();
        }
    }
}
