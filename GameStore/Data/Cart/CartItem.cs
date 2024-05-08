using System.ComponentModel.DataAnnotations;

namespace GameStore.Data.Cart
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }
        public Game Game { get; set; }
        public int Quantity { get; set; }
    }

}
