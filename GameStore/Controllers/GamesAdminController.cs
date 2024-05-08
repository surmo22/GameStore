using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GameStore.Data;

namespace GameStore.Controllers
{
    public class GamesAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GamesAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: GamesAdmin
        [HttpGet]
        [Route("[controller]/")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Games.ToListAsync());
        }

        // GET: GamesAdmin/Details/5
        [HttpGet]
        [Route("[controller]/Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // GET: GamesAdmin/Create
        [HttpGet]
        [Route("[controller]/Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: GamesAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("[controller]/Create")]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Price,ReleaseDate,Publisher,Developer,Platform,CoverImageUrl,TrailerUrl,GameImages")] Game game)
        {

            _context.Add(game);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: GamesAdmin/Edit/5
        [Route("[controller]/Edit/{id}")]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            return View(game);
        }

        // POST: GamesAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("[controller]/Edit/{id}")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Price,ReleaseDate,Publisher,Developer,Platform,CoverImageUrl,TrailerUrl,GameImages")] Game game)
        {
            if (id != game.Id)
            {
                return NotFound();
            }
            
            
            try
            {
                game.GameImages = game.GameImages[0].Split(',').ToList();
                _context.Update(game);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(game.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: GamesAdmin/Delete/5
        [Route("[controller]/Delete/{id}")]
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: GamesAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("[controller]/Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game != null)
            {
                _context.Games.Remove(game);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.Id == id);
        }
    }
}
