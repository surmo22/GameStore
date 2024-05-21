using GameStore.Data;
using GameStore.Services.GenreService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Controllers
{
    public class GamesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IGenreService genreService;
        private readonly GameService gameService;

        public GamesController(ApplicationDbContext context, IGenreService genreService, GameService gameService)
        {
            this._context = context;
            this.genreService = genreService;
            this.gameService = gameService;
        }

        // GET: Games/Index
        [HttpGet]
        public IActionResult Index()
        {
            var games = _context.Games.Take(20).ToList();
            return View(games);
        }
        public async Task<IActionResult> FetchGames()
        {
            await gameService.FetchAndSaveGamesAsync();
            return RedirectToAction("Index");
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

        // GET: Games/GamesByGenre/5
        [HttpGet]
        public async Task<IActionResult> GamesByGenre(int genreId)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }
            var games = await genreService.GetGamesByGenre(genreId);

            if (games == null)
            {
                return NotFound();
            }

            return View("Index", games);
        }
    }
}
