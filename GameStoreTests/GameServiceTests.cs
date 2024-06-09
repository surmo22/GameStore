using Xunit;
using Moq;
using GameStore.Services.GameService;
using GameStore.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Data.ViewModels;

namespace GameStoreTests
{
    public class GameServiceTests
    {
        private readonly ApplicationDbContext _mockContext;
        private readonly GameService _gameService;

        public GameServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "GameStore")
                .Options;

            _mockContext = new ApplicationDbContext(options);
            _gameService = new GameService(_mockContext);

        }

        [Fact]
        public async Task CreateGameAsync_ShouldCreateGame()
        {
            var game = new Game { Title = "Test Game" };
            var result = await _gameService.CreateGameAsync(game, "");

            Assert.NotNull(result);
            Assert.Equal("Test Game", result.Title);
        }

        [Fact]
        public async Task GetAllGamesAsync_ShouldReturnGames()
        {
            var game = new Game { Title = "Test Game" };
            await _gameService.CreateGameAsync(game, "");

            var result = await _gameService.GetAllGamesAsync(null, 1);

            Assert.NotNull(result);
            Assert.IsType<GamesViewModel>(result);
        }

        [Fact]
        public async Task GetGameByIdAsync_ShouldReturnGame()
        {
            // Arrange
            var game = new Game { Title = "Test Game" };
            await _gameService.CreateGameAsync(game, "");

            // Act
            var result = await _gameService.GetGameByIdAsync(game.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Game", result.Title);
        }

        [Fact]
        public async Task GetGamesByGenreAsync_ShouldReturnGames()
        {
            var result = await _gameService.GetGamesByGenreAsync(1, 1);

            Assert.NotNull(result);
            Assert.IsType<GamesViewModel>(result);
        }

        [Fact]
        public async Task EditGameAsync_ShouldEditGame()
        {
            var game = new Game { Id = 20, Title = "Test Game" };
            await _gameService.CreateGameAsync(game, "");

            var editedGame = new Game { Id = 20, Title = "Edited Game" };
            var result = await _gameService.EditGameAsync(1, editedGame, "");

            Assert.NotNull(result);
            Assert.Equal("Edited Game", result.Title);
        }
    }
}