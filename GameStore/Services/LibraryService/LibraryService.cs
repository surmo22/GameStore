using GameStore.Data;
using GameStore.Data.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Services.LibraryService
{
    public class LibraryService(ApplicationDbContext context) : ILibraryService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<IEnumerable<Key>> GetUserGameKeysAsync(IdentityUser user, int id)
        {
            var keys = await _context.OrderItems
                .Include(x => x.Game)
                .Include(x => x.Key)
                .Where(x => x.IdentityUser == user && x.Game.Id == id)
                .Select(x => x.Key)
                .ToListAsync();
            return keys;
        }

        public async Task<IEnumerable<Game>> GetUserGamesAsync(IdentityUser user)
        {
            var games = await _context.OrderItems
                .Include(x => x.Game)
                .Include(x => x.Key)
                .Where(x => x.IdentityUser == user)
                .Select(x => x.Game)
                .ToArrayAsync();
            return games;
        }
    }
}
