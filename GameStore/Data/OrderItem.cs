using Microsoft.AspNetCore.Identity;

namespace GameStore.Data
{
    public class OrderItem
    {
        public int Id { get; set; }
        public IdentityUser IdentityUser { get; set; }
        public Game Game { get; set; }
        public Key Key { get; set; }
    }
}
