using GameStore.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GameStore.Services.GenreService
{
    public class GenreService(ApplicationDbContext context) : IGenreService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<List<Genre>> GetAllGenresAsync()
        {
            return await _context.Genres.ToListAsync();
        }

        public async Task<Genre?> GetGenreByIdAsync(int id)
        {
            return await _context.Genres.Include(x => x.Games).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task CreateGenreAsync(Genre genre)
        {
            await _context.Genres.AddAsync(genre);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateGenreAsync(Genre genre)
        {
            _context.Entry(genre).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteGenreAsync(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre != null)
            {
                _context.Genres.Remove(genre);
                await _context.SaveChangesAsync();
            }
        }
    }
}
