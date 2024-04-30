using GameStore.Data.Cart;

namespace GameStore.Data.ViewModels
{
    public class CartViewModel
    {
        public string ReturnUrl { get; set; }
        public Cart.Cart Cart { get; set; }
    }
}