using GameStore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
        }
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Games");
        }
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
