using GameStore.Data;
using GameStore.Data.Cart;
using GameStore.Data.ViewModels;
using GameStore.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Controllers
{
    [Route("[controller]/")]
    public class CartController(ApplicationDbContext context, Cart cart) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        public Cart Cart { get; set; } = cart;

        [HttpGet]
        public IActionResult Index(string returnUrl)
        {
            return this.View(new CartViewModel
            {
                ReturnUrl = returnUrl ?? "/",
                Cart = this.HttpContext.Session.GetJson<Cart>("Cart") ?? new Cart(),
            });
        }

        [HttpPost]
        public IActionResult Index(long Id, string returnUrl)
        {
            Game? product = this._context.Games.FirstOrDefault(g => g.Id == Id);

            if (product != null)
            {
                this.Cart.AddItem(product, 1);

                return this.View(new CartViewModel
                {
                    Cart = this.Cart,
                    ReturnUrl = returnUrl,
                });
            }

            return this.RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Route("Cart/Remove")]
        public IActionResult Remove(long Id, string returnUrl)
        {
            this.Cart.RemoveLine(this.Cart.Lines.First(cl => cl.Game.Id == Id).Game);
            return this.View("Index", new CartViewModel
            {
                Cart = this.Cart,
                ReturnUrl = returnUrl ?? "/",
            });
        }
    }
}

