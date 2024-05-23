using GameStore.Data;
using GameStore.Data.ViewModels;
using GameStore.Services.GenreService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Controllers
{
    public class GamesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IGenreService genreService;
        private const int PageSize = 8;

        public GamesController(ApplicationDbContext context, IGenreService genreService)
        {
            this._context = context;
            this.genreService = genreService;
        }

        // GET: Games/Index
        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm = "", int page = 1)
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }
            var totalGames = await _context.Games.CountAsync();
            var totalPages = (int)Math.Ceiling(totalGames / (double)PageSize);
            IQueryable<Game> gamesQuery = _context.Games;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                gamesQuery = gamesQuery.Where(g => g.Title.Contains(searchTerm));
            }

            var games = await gamesQuery
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var viewModel = new GamesViewModel
            {
                Games = games,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(viewModel);
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
        public async Task<IActionResult> GamesByGenre(int genreId, int page = 1)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var games = await genreService.GetGamesByGenre(genreId);
            if (games == null)
            {
                return NotFound();
            }

            var totalGames = games.Count;
            var totalPages = (int)Math.Ceiling(totalGames / (double)PageSize);

            var pagedGames = games
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            var viewModel = new GamesViewModel
            {
                Games = pagedGames,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View("Index", viewModel);
        }
    }
}
