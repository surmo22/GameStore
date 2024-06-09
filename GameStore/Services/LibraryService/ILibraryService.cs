using GameStore.Data;
using GameStore.Data.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Services.LibraryService
{
    public interface ILibraryService
    {
        Task<Game?> GetLastBoughtGameAsync(IdentityUser? user);
        Task<IEnumerable<Key>> GetUserGameKeysAsync(IdentityUser user, int id);
        Task<IEnumerable<Game>> GetUserGamesAsync(IdentityUser user);
    }
}
