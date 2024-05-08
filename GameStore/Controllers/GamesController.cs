using GameStore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Controllers
{
    public class GamesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GamesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Games/Details/5
        [Route("~/Games/Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games.Include(g => g.Genres).FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }
    }
}
