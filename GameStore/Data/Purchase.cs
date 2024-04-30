using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Data
{
    public class Purchase
    {
        public int Id { get; set; }
        public required string UserId { get; set; } 
        public required IdentityUser User { get; set; } 
        public required int GameId { get; set; }
        public required Game Game { get; set; }
        public required DateTime PurchaseDate { get; set; }
    }
}
