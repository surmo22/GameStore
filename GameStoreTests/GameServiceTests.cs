using GameStore.Data;
using GameStore.Data.ViewModels;
using GameStore.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GameStoreTests
{
    public class GameServiceTests
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly GameService _gameService;

        public GameServiceTests()
        {
            var options = new DbContextOptions<ApplicationDbContext>();
            _mockContext = new Mock<ApplicationDbContext>(options);
            _gameService = new GameService(_mockContext.Object);
        }

        [Fact]
        public async Task GetAllGamesAsync_ReturnsGamesViewModel()
        {
            // Arrange
            var games = Enumerable.Range(1, 10).Select(i => new Game { Id = i }).AsQueryable();
            var mockSet = new Mock<DbSet<Game>>();
            mockSet.As<IQueryable<Game>>().Setup(m => m.Provider).Returns(games.Provider);
            mockSet.As<IQueryable<Game>>().Setup(m => m.Expression).Returns(games.Expression);
            mockSet.As<IQueryable<Game>>().Setup(m => m.ElementType).Returns(games.ElementType);
            mockSet.As<IQueryable<Game>>().Setup(m => m.GetEnumerator()).Returns(games.GetEnumerator());
            _mockContext.Setup(c => c.Games).Returns(mockSet.Object);

            // Act
            var result = await _gameService.GetAllGamesAsync(null, 1);

            // Assert
            Assert.Equal(8, result.Games.Count);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(2, result.TotalPages);
        }

        [Fact]
        public async Task GetGameByIdAsync_ReturnsGame()
        {
            // Arrange
            var game = new Game { Id = 1 };
            _mockContext.Setup(c => c.Games.FindAsync(1)).ReturnsAsync(game);

            // Act
            var result = await _gameService.GetGameByIdAsync(1);

            // Assert
            Assert.Equal(game, result);
        }

        [Fact]
        public async Task GetGamesByGenreAsync_ReturnsGamesViewModel()
        {
            // Arrange
            var genres = Enumerable.Range(1, 10).Select(i => new Genre { Id = i, Games = Enumerable.Range(1, 10).Select(j => new Game { Id = j }).ToList() }).AsQueryable();
            var mockSet = new Mock<DbSet<Genre>>();
            mockSet.As<IQueryable<Genre>>().Setup(m => m.Provider).Returns(genres.Provider);
            mockSet.As<IQueryable<Genre>>().Setup(m => m.Expression).Returns(genres.Expression);
            mockSet.As<IQueryable<Genre>>().Setup(m => m.ElementType).Returns(genres.ElementType);
            mockSet.As<IQueryable<Genre>>().Setup(m => m.GetEnumerator()).Returns(genres.GetEnumerator());
            _mockContext.Setup(c => c.Genres).Returns(mockSet.Object);

            // Act
            var result = await _gameService.GetGamesByGenreAsync(1, 1);

            // Assert
            Assert.Equal(8, result.Games.Count);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(2, result.TotalPages);
        }
    }
}
