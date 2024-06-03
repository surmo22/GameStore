using System.Collections.Generic;
using GameStore.Data;
using GameStore.Services.LibraryService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GameStoreTests
{
    public class LibraryServiceTests
    {
        private readonly Mock<DbSet<OrderItem>> _mockOrderItems;
        private readonly LibraryService _service;

        public LibraryServiceTests()
        {
            // Set up data
            var orderItems = new List<OrderItem>
        {
            new OrderItem { IdentityUser = new IdentityUser("User1"), Game = new Game { Id = 1 }, Key = new Key { Value = "Key1" } },
            new OrderItem { IdentityUser = new IdentityUser("User1"), Game = new Game { Id = 2 }, Key = new Key { Value = "Key2" } },
            new OrderItem { IdentityUser = new IdentityUser("User2"), Game = new Game { Id = 1 }, Key = new Key { Value = "Key3" } }
        }.AsQueryable();

            // Set up mock DbSet
            _mockOrderItems = new Mock<DbSet<OrderItem>>();
            _mockOrderItems.As<IQueryable<OrderItem>>().Setup(m => m.Provider).Returns(orderItems.Provider);
            _mockOrderItems.As<IQueryable<OrderItem>>().Setup(m => m.Expression).Returns(orderItems.Expression);
            _mockOrderItems.As<IQueryable<OrderItem>>().Setup(m => m.ElementType).Returns(orderItems.ElementType);
            _mockOrderItems.As<IQueryable<OrderItem>>().Setup(m => m.GetEnumerator()).Returns(orderItems.GetEnumerator());

            // Set up mock DbContext
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.OrderItems).Returns(_mockOrderItems.Object);

            // Create service
            _service = new LibraryService(mockContext.Object);
        }

        [Fact]
        public async Task GetUserGameKeysAsync_ReturnsCorrectKeys()
        {
            // Arrange
            var user = new IdentityUser("User1");
            var gameId = 1;

            // Act
            var keys = await _service.GetUserGameKeysAsync(user, gameId);

            // Assert
            Assert.Single(keys);
            Assert.Equal("Key1", keys.First().Value);
        }

        [Fact]
        public async Task GetUserGamesAsync_ReturnsCorrectGames()
        {
            // Arrange
            var user = new IdentityUser("User1");

            // Act
            var games = await _service.GetUserGamesAsync(user);

            // Assert
            Assert.Equal(2, games.Count());
        }
    }
}