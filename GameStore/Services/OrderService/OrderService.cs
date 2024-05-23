using GameStore.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Services.OrderService
{
    public class OrderService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager) : IOrderService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task AddOrderAsync(Order order, Data.Cart.CartItem[] cartItems)
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User 
                ?? throw new InvalidOperationException("Couldn't get user")) 
                ?? throw new InvalidOperationException("Couldn't find user");
            foreach (var cartItem in cartItems)
            {
                var keys = await _context.Keys.Include(x => x.Game).Where(x => x.Game.Id == cartItem.Game.Id).ToArrayAsync();
                for (int i = 0; i < cartItem.Quantity; i++)
                {
                    var key = Array.Find(keys, (k) => !k.Activated);
                    var game = await _context.Games.FindAsync(cartItem.Game.Id)
                   ?? throw new InvalidOperationException("Game not found");
                    if (key == null)
                    {
                        key = new Key
                        {
                            Game = game,
                            Value = "MyKey",
                            Activated = true,
                        };
                        await _context.Keys.AddAsync(key);
                        await _context.SaveChangesAsync();
                    }
                    key.Activated = true;

                    order.Lines.Add(new OrderItem()
                    {
                        Game = game,
                        IdentityUser = currentUser,
                        Key = key,
                    });
                }
            }
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }
    }
}
