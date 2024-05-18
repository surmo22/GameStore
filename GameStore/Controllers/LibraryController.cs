using GameStore.Services.LibraryService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Controllers
{
    [Authorize]
    public class LibraryController : Controller
    {
        private readonly ILibraryService libraryService;
        private readonly UserManager<IdentityUser> userManager;

        public LibraryController(ILibraryService libraryService, UserManager<IdentityUser> userManager)
        {
            this.libraryService = libraryService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(this.User) ?? throw new InvalidOperationException("Couldn't find user");
            var games = await libraryService.GetUserGamesAsync(user);
            return View(games);
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await userManager.GetUserAsync(this.User) ?? throw new InvalidOperationException("Couldn't find user");
            var keys = await libraryService.GetUserGameKeysAsync(user, id);
            return View(keys);
        }
    }
}
