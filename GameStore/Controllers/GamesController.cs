using GameStore.Data;
using GameStore.Data.ViewModels;
using GameStore.Services.GenreService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Controllers
{
    public class GamesController : Controller
    {
        private readonly IGameService _gameService;

        public GamesController(IGameService gameService)
        {
            _gameService = gameService;
        }

        public async Task<IActionResult> Index(string? searchTerm, int page = 1)
        {
            if (ModelState.IsValid)
            {
                return View(await _gameService.GetAllGamesAsync(searchTerm, page));
            }
            return BadRequest();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var game = await _gameService.GetGameByIdAsync(id.Value);
                if (game == null)
                {
                    return NotFound();
                }
                return View(game);

            }
            return BadRequest();
        }

        public async Task<IActionResult> GamesByGenre(int? genreId, int page = 1)
        {
            if (genreId == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid) {
                return View("Index", await _gameService.GetGamesByGenreAsync(genreId.Value, page));
            }
            return BadRequest();
        }
    }
}
