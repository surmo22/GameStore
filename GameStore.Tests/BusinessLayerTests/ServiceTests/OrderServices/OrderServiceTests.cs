using AutoMapper;
using GameStore.BLL.DTOs.Games;
using GameStore.BLL.DTOs.Orders;
using GameStore.BLL.DTOs.Payment;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.BLL.Interfaces.NotificationServices;
using GameStore.BLL.Interfaces.OrderServices;
using GameStore.BLL.Interfaces.PaymentServices;
using GameStore.BLL.Interfaces.Repositories;
using GameStore.BLL.Services.OrderServices;
using GameStore.Common.Constants;
using GameStore.Common.Exceptions;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.Entities.Relations;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.OrderServices;

public class OrderServiceTests
{
    private readonly Mock<IOrderManager> _orderManagerMock = new();
    private readonly Mock<IGameService> _gameServiceMock = new();
    private readonly Mock<ICartManager> _cartManagerMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IPaymentService> _paymentServiceMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IDateTimeProvider> _timeMock = new();
    private readonly Mock<ILogger<OrderService>> _loggerMock = new();
    private readonly Mock<INotificationService> _notificationServiceMock = new();

    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        var orderServices =
            new BLL.Services.OrderServices.OrderServices(_orderManagerMock.Object, _cartManagerMock.Object,
                _paymentServiceMock.Object);
        _orderService = new OrderService(
            orderServices,
            _gameServiceMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _timeMock.Object,
            _notificationServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task AddGameToCartAsync_AddsGameSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var gameKey = "game-key";
        var game = new GameDto();
        var order = new Order { Id = Guid.NewGuid(), Items = [] };

        _gameServiceMock.Setup(x => x.GetGameByKeyAsync(gameKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(game);
        _orderManagerMock.Setup(x => x.GetOrCreateOpenOrderAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _unitOfWorkMock.Setup(x => x.Orders.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()));
        _unitOfWorkMock.Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>()));

        // Act
        await _orderService.AddGameToCartAsync(userId, gameKey, CancellationToken.None);

        // Assert
        _cartManagerMock.Verify(x => x.AddGameToOrder(order, game), Times.Once);
        _unitOfWorkMock.Verify(x => x.Orders.UpdateAsync(order, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CompleteAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveGameFromCartAsync_RemovesGameSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var gameKey = "game-key";
        var game = new GameDto();
        var order = new Order { Id = Guid.NewGuid(), Items = [] };

        _gameServiceMock.Setup(x => x.GetGameByKeyAsync(gameKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(game);
        _unitOfWorkMock.Setup(x => x.Orders.GetCustomersOpenOrderAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _unitOfWorkMock.Setup(x => x.Orders.UpdateAsync(order, It.IsAny<CancellationToken>()));
        _unitOfWorkMock.Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>()));
        _orderManagerMock.Setup(x => x.DeleteOrderIfEmptyAsync(order, It.IsAny<CancellationToken>()));

        // Act
        await _orderService.RemoveGameFromCartAsync(userId, gameKey, CancellationToken.None);

        // Assert
        _cartManagerMock.Verify(x => x.RemoveGameFromOrder(order, game), Times.Once);
        _unitOfWorkMock.Verify(x => x.Orders.UpdateAsync(order, It.IsAny<CancellationToken>()), Times.Once);
        _orderManagerMock.Verify(x => x.DeleteOrderIfEmptyAsync(order, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CompleteAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPaidAndCancelledOrdersAsync_ReturnsMappedOrders()
    {
        // Arrange
        List<Order> orders = [new()];
        List<OrderDto> orderDtos = [new()];
        _unitOfWorkMock.Setup(x => x.Orders.GetPaidAndCancelledOrdersAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(orders);
        _mapperMock.Setup(m => m.Map<OrderDto>(It.IsAny<Order>())).Returns(orderDtos[0]);

        // Act
        var result = await _orderService.GetPaidAndCancelledOrdersAsync(CancellationToken.None);

        // Assert
        Assert.Equal(orderDtos.Count, result.Count);
        Assert.All(result, dto => Assert.Contains(dto, orderDtos));
    }

    [Fact]
    public async Task GetOrderByIdAsync_ReturnsMappedOrder()
    {
        // Arrange
        var order = new Order();
        var orderDto = new OrderDto();
        _unitOfWorkMock.Setup(x => x.Orders.GetOrderByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(order);
        _mapperMock.Setup(m => m.Map<OrderDto>(It.IsAny<Order>())).Returns(orderDto);

        // Act
        var result = await _orderService.GetOrderByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Equal(orderDto, result);
    }

    [Fact]
    public async Task GetOrderDetailsByIdAsync_ThrowsIfOrderNotFound()
    {
        // Arrange
        _unitOfWorkMock.Setup(x => x.Orders.GetOrderByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync((Order)null!);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _orderService.GetOrderDetailsByIdAsync(Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task GetOrderDetailsByIdAsync_ReturnsOrderGameDtos()
    {
        // Arrange
        var order = new Order { Items = [new()] };
        List<OrderGameDto> orderGameDtos = [new()];
        _unitOfWorkMock.Setup(x => x.Orders.GetOrderByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(order);
        _mapperMock.Setup(m => m.Map<OrderGameDto>(It.IsAny<OrderGame>())).Returns(orderGameDtos[0]);

        // Act
        var result = await _orderService.GetOrderDetailsByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Equal(orderGameDtos.Count, result.Count);
        Assert.All(result, dto => Assert.Contains(dto, orderGameDtos));
    }

    [Fact]
    public async Task GetUserOpenOrderAsync_ReturnsEmptyListIfOrderNull()
    {
        // Arrange
        _unitOfWorkMock.Setup(x => x.Orders.GetCustomersOpenOrderAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync((Order)null!);

        // Act
        var result = await _orderService.GetUserOpenOrderAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserOpenOrderAsync_ReturnsOrderGameDtos()
    {
        // Arrange
        var order = new Order { Items = [new()] };
        List<OrderGameDto> orderGameDtos = [new()];
        _unitOfWorkMock.Setup(x => x.Orders.GetCustomersOpenOrderAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(order);
        _mapperMock.Setup(m => m.Map<OrderGameDto>(It.IsAny<OrderGame>())).Returns(orderGameDtos[0]);

        // Act
        var result = await _orderService.GetUserOpenOrderAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Equal(orderGameDtos.Count, result.Count);
        Assert.All(result, dto => Assert.Contains(dto, orderGameDtos));
    }

    [Fact]
    public async Task PayForOrderAsync_SuccessfullyPaysAndUpdatesOrder()
    {
        // Arrange
        var paymentRequest = new PaymentRequest();
        var order = new Order { Id = Guid.NewGuid(), Status = OrderStatuses.Open, Items = new List<OrderGame>()
        {
            new ()
            {
                ProductId = Guid.Empty,
                Product = new ()
                {
                    UnitInStock = 20,
                },
            },
        }};
        _unitOfWorkMock.Setup(x => x.Orders.GetCustomersOpenOrderAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        var paymentResultMock = new Mock<IPaymentResult>();
        _paymentServiceMock.Setup(x => x.PayForOrderAsync(order, paymentRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentResultMock.Object);

        // Act
        var result = await _orderService.PayForOrderAsync(It.IsAny<Guid>(), paymentRequest, CancellationToken.None);

        // Assert
        Assert.Equal(paymentResultMock.Object, result);
        Assert.Equal(OrderStatuses.Paid, order.Status);
        _unitOfWorkMock.Verify(x => x.Orders.UpdateAsync(order, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CompleteAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task PayForOrderAsync_FailsAndCancelsOrder()
    {
        // Arrange
        var paymentRequest = new PaymentRequest();
        var order = new Order { Id = Guid.NewGuid(), Status = OrderStatuses.Open };
        _unitOfWorkMock.Setup(x => x.Orders.GetCustomersOpenOrderAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _paymentServiceMock.Setup(x => x.PayForOrderAsync(order, paymentRequest, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Payment failed"));

        // Act & Assert
        await Assert.ThrowsAsync<PaymentFailedException>(() =>
            _orderService.PayForOrderAsync(It.IsAny<Guid>(), paymentRequest, CancellationToken.None));

        Assert.Equal(OrderStatuses.Cancelled, order.Status);
        _unitOfWorkMock.Verify(x => x.Orders.UpdateAsync(order, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CompleteAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task ShipOrderAsync_ValidPaidOrder_ChangesStatusToShipped()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Order { Id = orderId, Status = OrderStatuses.Paid };

        _unitOfWorkMock.Setup(u => u.Orders.GetOrderByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        await _orderService.ShipOrderAsync(orderId, CancellationToken.None);

        // Assert
        Assert.Equal(OrderStatuses.Shipped, order.Status);
        _unitOfWorkMock.Verify(u => u.Orders.UpdateAsync(order, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CompleteAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShipOrderAsync_OrderNotFound_ThrowsKeyNotFound()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.Orders.GetOrderByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order)null);

        // Act && Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _orderService.ShipOrderAsync(orderId, CancellationToken.None));
    }

    [Fact]
    public async Task ShipOrderAsync_OrderNotPaid_ThrowsInvalidOperation()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Order { Id = orderId, Status = OrderStatuses.Cancelled };

        _unitOfWorkMock.Setup(u => u.Orders.GetOrderByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        
        // Act && Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _orderService.ShipOrderAsync(orderId, CancellationToken.None));
    }
    
    [Fact]
    public async Task ShipOrderAsync_OrderShipped_ReturnsNothing()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Order { Id = orderId, Status = OrderStatuses.Shipped };

        _unitOfWorkMock.Setup(u => u.Orders.GetOrderByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        await _orderService.ShipOrderAsync(orderId, CancellationToken.None);
        
        // Assert
        _unitOfWorkMock.Verify(u => u.Orders.UpdateAsync(order, It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CompleteAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteOrderItem_RemovesItem_AndCancelsIfEmpty()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var item = new OrderGame { Id = itemId };
        var order = new Order { Items = new List<OrderGame> { item }, Status = OrderStatuses.Paid };

        _unitOfWorkMock.Setup(u => u.Orders.GetOrderByItemIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        await _orderService.DeleteOrderItem(itemId, CancellationToken.None);

        // Assert
        Assert.Empty(order.Items);
        Assert.Equal(OrderStatuses.Cancelled, order.Status);
        _unitOfWorkMock.Verify(u => u.Orders.UpdateAsync(order, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateOrderDetailsQuantity_UpdatesQuantity()
    {
        // Act
        var itemId = Guid.NewGuid();
        var item = new OrderGame { Id = itemId, Quantity = 1 };
        var order = new Order { Items = new List<OrderGame> { item } };

        _unitOfWorkMock.Setup(u => u.Orders.GetOrderByItemIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Arrange
        await _orderService.UpdateOrderDetailsQuantity(itemId, 5, CancellationToken.None);

        // Assert
        Assert.Equal(5, item.Quantity);
        _unitOfWorkMock.Verify(u => u.Orders.UpdateAsync(order, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task AddGameToOrder_ValidInputs_AddsGameToOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var key = "game-123";
        var order = new Order();
        var game = new GameDto();
        
        _unitOfWorkMock.Setup(u => u.Orders.GetOrderByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _gameServiceMock.Setup(g => g.GetGameByKeyAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(game);

        // Act
        await _orderService.AddGameToOrder(orderId, key, CancellationToken.None);

        // Assert
        _cartManagerMock.Verify(m => m.AddGameToOrder(order, game), Times.Once);
        _unitOfWorkMock.Verify(u => u.Orders.UpdateAsync(order, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetOrderHistory_ReturnsMappedDtos()
    {
        // Arrange
        var start = DateTime.UtcNow.AddDays(-10);
        var end = DateTime.UtcNow;

        var orders = new List<Order>
        {
            new() { Id = Guid.NewGuid(), }
        };
        
        // Act
        _mapperMock.Setup(m => m.Map<OrderDto>(It.IsAny<Order>())).Returns(new OrderDto());
        _unitOfWorkMock.Setup(u => u.Orders.GetAllOrdersByDate(start, end, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);

        var result = await _orderService.GetOrderHistory(start, end, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.IsType<OrderDto>(result.First());
    }
}