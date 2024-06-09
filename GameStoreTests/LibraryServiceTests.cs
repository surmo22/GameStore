using Xunit;
using GameStore.Services.LibraryService;
using GameStore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace GameStoreTests
{
    public class LibraryServiceTests
    {
        private readonly ApplicationDbContext _mockContext;
        private readonly LibraryService _libraryService;
        private readonly IdentityUser _testUser;

        public LibraryServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "LibraryStore")
                .Options;

            _mockContext = new ApplicationDbContext(options);
            _libraryService = new LibraryService(_mockContext);
            _testUser = new IdentityUser { UserName = "TestUser", Email = "testuser@example.com" };
        }

        [Fact]
        public async Task GetUserGameKeysAsync_ShouldReturnKeys()
        {
            var game = new Game { Title = "Test Game" };
            var key = new Key { Game = game };
            _mockContext.OrderItems.Add(new OrderItem { Game = game, Key = key, IdentityUser = _testUser });
            await _mockContext.SaveChangesAsync();

            var result = await _libraryService.GetUserGameKeysAsync(_testUser, game.Id);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(key, result.First());
        }

        [Fact]
        public async Task GetUserGamesAsync_ShouldReturnGames()
        {
            var game = new Game { Title = "Test Game" };
            var key = new Key { Game = game };
            _mockContext.OrderItems.Add(new OrderItem { Game = game, Key = key, IdentityUser = _testUser });
            await _mockContext.SaveChangesAsync();

            var result = await _libraryService.GetUserGamesAsync(_testUser);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(game, result.First());
        }
    }
}
