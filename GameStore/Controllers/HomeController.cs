using GameStore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var games = _context.Games.Take(20).ToList();
            return View(games);
        }
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
