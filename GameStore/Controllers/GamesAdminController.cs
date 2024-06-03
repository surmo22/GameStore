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
        private readonly IGameService gameService;

        public GamesAdminController(ApplicationDbContext context, IGenreService genreService, IGameService gameService)
        {
            this._context = context;
            this.genreService = genreService;
            this.gameService = gameService;
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
        public async Task<IActionResult> Create()
        {
            var genres = await genreService.GetAllGenresAsync();
            ViewBag.Genres = new SelectList(genres, "Id", "Name");
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
                }
                game.Genres = genres;
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
                    }
                    var foundGame = await _context.Games
                                        .Include(g => g.Genres)
                                        .FirstOrDefaultAsync(g => g.Id == game.Id);

                    if (foundGame == null)
                    {
                        return NotFound();
                    }

                    foundGame.Title = game.Title;
                    foundGame.Description = game.Description;
                    foundGame.Price = game.Price;
                    foundGame.ReleaseDate = game.ReleaseDate;
                    foundGame.Publisher = game.Publisher;
                    foundGame.Developer = game.Developer;
                    foundGame.Platform = game.Platform;
                    foundGame.CoverImageUrl = game.CoverImageUrl;
                    foundGame.TrailerUrl = game.TrailerUrl;
                    foundGame.GameImages = game.GameImages;
                    if (genres.Count > 0)
                    {
                        foundGame.Genres?.Clear();
                        foreach (var genre in genres)
                        {
                            foundGame.Genres?.Add(genre);
                        }
                    }
                    _context.Update(foundGame);
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

        public IActionResult AddKeys(int id)
        {
            var viewModel = new AddKeysViewModel
            {
                GameId = id
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddKeys(int gameId, string keys)
        {
            var keyValues = keys.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var keyValue in keyValues)
            {
                var key = new Key
                {
                    Value = keyValue,
                    Game = await gameService.GetGameByIdAsync(gameId),
                };
                await _context.Keys.AddAsync(key);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = gameId });
        }

        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.Id == id);
        }
    }
}
