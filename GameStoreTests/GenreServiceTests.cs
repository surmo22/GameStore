using Xunit;
using GameStore.Services.GenreService;
using GameStore.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace GameStoreTests
{
    public class GenreServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _mockContext;
        private readonly GenreService _genreService;

        public GenreServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "GenreStore")
                .Options;

            _mockContext = new ApplicationDbContext(options);
            _genreService = new GenreService(_mockContext);
        }

        public void Dispose()
        {
            _mockContext.Database.EnsureDeleted(); // Delete the database after each test
        }

        [Fact]
        public async Task GetAllGenresAsync_ShouldReturnGenres()
        {
            var genre = new Genre { Name = "Test Genre" };
            _mockContext.Genres.Add(genre);
            await _mockContext.SaveChangesAsync();

            var result = await _genreService.GetAllGenresAsync();

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Test Genre", result.First().Name);
        }

        [Fact]
        public async Task GetGenreByIdAsync_ShouldReturnGenre()
        {
            var genre = new Genre { Name = "Test Genre" };
            _mockContext.Genres.Add(genre);
            await _mockContext.SaveChangesAsync();

            var result = await _genreService.GetGenreByIdAsync(genre.Id);

            Assert.NotNull(result);
            Assert.Equal("Test Genre", result.Name);
        }

        [Fact]
        public async Task CreateGenreAsync_ShouldCreateGenre()
        {
            var genre = new Genre { Name = "Test Genre" };

            await _genreService.CreateGenreAsync(genre);

            Assert.Single(_mockContext.Genres);
            Assert.Equal("Test Genre", _mockContext.Genres.First().Name);
        }

        [Fact]
        public async Task UpdateGenreAsync_ShouldUpdateGenre()
        {
            var genre = new Genre { Name = "Test Genre" };
            _mockContext.Genres.Add(genre);
            await _mockContext.SaveChangesAsync();

            genre.Name = "Updated Genre";
            await _genreService.UpdateGenreAsync(genre);

            Assert.Equal("Updated Genre", _mockContext.Genres.First().Name);
        }

        [Fact]
        public async Task DeleteGenreAsync_ShouldDeleteGenre()
        {
            var genre = new Genre { Name = "Test Genre" };
            _mockContext.Genres.Add(genre);
            await _mockContext.SaveChangesAsync();

            await _genreService.DeleteGenreAsync(genre.Id);

            Assert.Empty(_mockContext.Genres);
        }

        [Fact]
        public async Task GetGamesByGenre_ShouldReturnGames()
        {
            var game = new Game { Title = "Test Game" };
            var genre = new Genre { Name = "Test Genre", Games = new List<Game> { game } };
            _mockContext.Genres.Add(genre);
            await _mockContext.SaveChangesAsync();

            var result = await _genreService.GetGamesByGenre(genre.Id);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Test Game", result.First().Title);
        }
    }
}
