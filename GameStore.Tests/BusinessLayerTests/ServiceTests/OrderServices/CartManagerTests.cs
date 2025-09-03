using GameStore.BLL.DTOs.Games;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Services.OrderServices;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.Entities.Relations;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.OrderServices;

public class CartManagerTests
{
    private readonly Mock<IGuidProvider> _guidProvider = new();
    private readonly CartManager _cartManager;

    public CartManagerTests()
    {
        _guidProvider.Setup(g => g.NewGuid()).Returns(Guid.NewGuid);
        _cartManager = new CartManager(_guidProvider.Object);
    }
    
    [Fact]
    public void AddGameToOrder_AddsNewGame_WhenNotAlreadyPresent()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var order = new Order { Id = guid, Items = [] };
        var game = new GameDto { Id = guid, Price = 100, Discount = 10 };

        // Act
        _cartManager.AddGameToOrder(order, game);

        // Assert
        var orderItem = order.Items.Single();
        Assert.Equal(guid, orderItem.ProductId);
        Assert.Equal(1, orderItem.Quantity);
        Assert.Equal(100, orderItem.Price);
        Assert.Equal(10, orderItem.Discount);
    }

    [Fact]
    public void AddGameToOrder_IncreasesQuantity_WhenAlreadyPresent()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var order = new Order { Id = guid, Items = [new OrderGame { ProductId = guid, Quantity = 1 }] };
        var game = new GameDto { Id = guid, Price = 100, Discount = 10 };

        // Act
        _cartManager.AddGameToOrder(order, game);

        // Assert
        Assert.Equal(2, order.Items.Single().Quantity);
    }

    [Fact]
    public void RemoveGameFromOrder_DecreasesQuantity_WhenMoreThanOneExists()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var order = new Order { Id = guid, Items = [new OrderGame { ProductId = guid, Quantity = 3 }] };
        var game = new GameDto { Id = guid };

        // Act
        _cartManager.RemoveGameFromOrder(order, game);

        // Assert
        Assert.Equal(2, order.Items.Single().Quantity);
    }

    [Fact]
    public void RemoveGameFromOrder_RemovesGame_WhenQuantityIsOne()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var order = new Order { Id = guid, Items = [new OrderGame { ProductId = guid, Quantity = 1 }] };
        var game = new GameDto { Id = guid };

        // Act
        _cartManager.RemoveGameFromOrder(order, game);

        // Assert
        Assert.Empty(order.Items);
    }

    [Fact]
    public void RemoveGameFromOrder_DoesNothing_WhenGameNotInOrder()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var order = new Order { Id = guid, Items = [] };
        var game = new GameDto { Id = guid };

        // Act
        _cartManager.RemoveGameFromOrder(order, game);

        // Assert
        Assert.Empty(order.Items);
    }
}