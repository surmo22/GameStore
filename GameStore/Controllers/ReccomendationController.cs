using GameStore.Data;
using GameStore.Data.ViewModels;
using GameStore.Services.GenreService;
using GameStore.Services.ReccomendationService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameStore.Controllers
{
    public class ReccomendationController : Controller
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly IReccomendationService reccomendationService;
        private readonly IGenreService genreService;

        public ReccomendationController(ApplicationDbContext applicationDbContext, IReccomendationService reccomendationService, IGenreService genreService)
        {
            this.applicationDbContext = applicationDbContext;
            this.reccomendationService = reccomendationService;
            this.genreService = genreService;
        }

        public async Task<IActionResult> Index()
        {
            var allGenres = await genreService.GetAllGenresAsync();
            var allGames = await applicationDbContext.Games.ToListAsync();
            var reccomendationViewModel = new ReccomendationViewModel() 
            { 
                Games = allGames,
                Genres = allGenres,
                SelectedGenreIds = new List<int>(),
            };
            return View(reccomendationViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Recommend(ReccomendationViewModel model)
        {
            var allGames = await applicationDbContext.Games.Include(g => g.Genres).ToListAsync();


            IList<Game> recommendedGames = new List<Game>();

            if (model.SelectedGameId.HasValue)
            {
                var selectedGame = allGames.Find(g => g.Id == model.SelectedGameId.Value);
                if (selectedGame != null)
                {
                    var genres = selectedGame.Genres?.ToList();
                    model.SelectedGenreIds = genres?.Select(x=>x.Id).ToList();
                    recommendedGames = await reccomendationService.GetReccomendedGames(genres);
                }
            }
            else if (model.SelectedGenreIds != null && model.SelectedGenreIds.Count > 0)
            {
                var selectedGenres = model.SelectedGenreIds;
                IList<Genre> genres = new List<Genre>();
                foreach (var item in selectedGenres)
                {
                    genres.Add(await genreService.GetGenreByIdAsync(item));
                }
                recommendedGames = await reccomendationService.GetReccomendedGames(genres);
            }

            var allGenres = await genreService.GetAllGenresAsync();
            model.Games = allGames;
            model.Genres = allGenres;
            model.RecommendedGames = recommendedGames.Take(8).ToList();
            return View("Index", model);
        }
    }
}
