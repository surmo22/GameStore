using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GameStore.Data;
using GameStore.Services.GenreService;
using Microsoft.AspNetCore.Authorization;

namespace GameStore.Controllers
{
    [Route("[controller]")]
    [Authorize(Roles = "Admin")]
    public class GamesAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IGenreService genreService;

        public GamesAdminController(ApplicationDbContext context, IGenreService genreService)
        {
            this._context = context;
            this.genreService = genreService;
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

            var game = await _context.Games.Include(x => x.Genres)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (game == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                return View(game);
            }
            return RedirectToAction("Index");
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
            if (ModelState.IsValid) { 
                await _context.AddAsync(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(game);
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
            if (ModelState.IsValid)
            {
                var game = await _context.Games.Include(x => x.Genres).FirstOrDefaultAsync(x => x.Id == id);
                var genres = await genreService.GetAllGenresAsync();
                if (game == null)
                {
                    return NotFound();
                }
                ViewBag.Genres = new SelectList(genres, "Id", "Name");
                return View(game);
            }
            return RedirectToPage("Error");
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
                if (ModelState.IsValid)
                {
                    var selectedGenres = Request.Form["Genres"].ToString();
                    IList<Genre> genres = new List<Genre>();
                    if (!string.IsNullOrEmpty(selectedGenres))
                    {
                        var genresIds = selectedGenres.Split(',').Select(int.Parse).ToList();
                        foreach (var genre in genresIds)
                        {
                            var x = await genreService.GetGenreByIdAsync(genre);
                            if (x != null)
                            {
                                genres.Add(x);
                            }
                        }
                        var foundGame = await _context.Games.FindAsync(game.Id);
                        if (foundGame != null)
                        {
                            _context.Games.Remove(foundGame);
                        }
                    }
                    game = new Game
                    {
                        Id = id,
                        Title = game.Title,
                        Description = game.Description,
                        Price = game.Price,
                        ReleaseDate = game.ReleaseDate,
                        Publisher = game.Publisher,
                        Developer = game.Developer,
                        Platform = game.Platform,
                        CoverImageUrl = game.CoverImageUrl,
                        TrailerUrl = game.TrailerUrl,
                        GameImages = game.GameImages[0].Split(',').ToList(),
                        Genres = genres,
                    };
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
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
            if (ModelState.IsValid)
            {
                var game = await _context.Games
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (game == null)
                {
                    return NotFound();
                }

                return View(game);
            }
            return RedirectToPage("Error");
        }

        // POST: GamesAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("[controller]/Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (ModelState.IsValid)
            {
                var game = await _context.Games.FindAsync(id);
                if (game != null)
                {
                    _context.Games.Remove(game);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.Id == id);
        }
    }
}
