using System.Security.Claims;
using FluentAssertions;
using GameStore.BLL.DTOs.Orders;
using GameStore.BLL.DTOs.Payment;
using GameStore.BLL.DTOs.PaymentMethods;
using GameStore.BLL.Interfaces.OrderServices;
using GameStore.BLL.Interfaces.PaymentServices;
using GameStore.Common.Constants;
using GameStore.WebApi.Controllers;
using GameStore.WebApi.PaymentResultHandler;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Tests.ApiTests;

public class OrdersControllerTests
{
    private readonly Mock<IOrderService> _mockOrderService;
    private readonly Mock<IPaymentMethodsService> _mockPaymentMethodsService;
    private readonly Mock<IPaymentResultHandler> _mockPaymentResultHandler;
    private readonly OrdersController _controller;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public OrdersControllerTests()
    {
        _mockOrderService = new Mock<IOrderService>();
        _mockPaymentMethodsService = new Mock<IPaymentMethodsService>();
        _mockPaymentResultHandler = new();
        _controller = new OrdersController(_mockOrderService.Object, _mockPaymentMethodsService.Object, _mockPaymentResultHandler.Object);
        var userId = Guid.NewGuid();
        var user = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        ], "mock"));
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext() { User = user}
        };
    }
    
    [Fact]
    public async Task GetHistory_ReturnsOkWithOrders()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var startDate = new DateTime(2023, 1, 1);
        var endDate = new DateTime(2023, 12, 31);

        var orderList = new List<OrderDto> { new(), new() };
        _mockOrderService
            .Setup(s => s.GetOrderHistory(startDate, endDate, cancellationToken))
            .ReturnsAsync(orderList);

        // Act
        var result = await _controller.GetHistory(startDate, endDate, cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedOrders = Assert.IsAssignableFrom<IEnumerable<OrderDto>>(okResult.Value);
        Assert.Equal(2, ((List<OrderDto>)returnedOrders).Count);
    }

    [Fact]
    public async Task UpdateQuantity_ReturnsOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateQuantityRequest { Count = 5 };
        var cancellationToken = CancellationToken.None;

        _mockOrderService
            .Setup(s => s.UpdateOrderDetailsQuantity(id, request.Count, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateQuantity(id, request, cancellationToken);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteOrderItem_ReturnsOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        _mockOrderService
            .Setup(s => s.DeleteOrderItem(id, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteOrderItem(id, cancellationToken);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task AddGameToOrder_ReturnsOk()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var gameKey = "game-key";
        var cancellationToken = CancellationToken.None;

        _mockOrderService
            .Setup(s => s.AddGameToOrder(orderId, gameKey, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AddGameToOrder(orderId, gameKey, cancellationToken);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task ShipOrder_ReturnsOk()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        _mockOrderService
            .Setup(s => s.ShipOrderAsync(orderId, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ShipOrder(orderId, cancellationToken);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task GetPaidAndCancelledOrders_ReturnsOk_WithOrders()
    {
        // Arrange
        var fakeOrders = new List<OrderDto> { new(), new() };
        _mockOrderService.Setup(x => x.GetPaidAndCancelledOrdersAsync(_cancellationToken))
            .ReturnsAsync(fakeOrders);

        // Act
        var result = await _controller.GetPaidAndCancelledOrders(_cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var ordersReturn = Assert.IsAssignableFrom<List<OrderDto>>(okResult.Value);
        Assert.Equal(fakeOrders.Count, ordersReturn.Count);
    }

    [Fact]
    public async Task GetOrderById_ReturnsOk_WithOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var fakeOrder = new OrderDto { Id = orderId };
        _mockOrderService.Setup(x => x.GetOrderByIdAsync(orderId, _cancellationToken))
            .ReturnsAsync(fakeOrder);

        // Act
        var result = await _controller.GetOrderById(orderId, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var orderReturned = Assert.IsAssignableFrom<OrderDto>(okResult.Value);
        Assert.Equal(fakeOrder.Id, orderReturned.Id);
    }

    [Fact]
    public async Task RemoveGameFromCart_ReturnsNoContent()
    {
        // Arrange
        var gameKey = "gameKey";
        _mockOrderService.Setup(x => x.RemoveGameFromCartAsync(CustomerStub.CustomerId, gameKey, _cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.RemoveGameFromCart(gameKey, _cancellationToken);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task GetPaymentMethods_ReturnsOk_WithMethods()
    {
        // Arrange
        var fakePaymentMethods = new PaymentMethodList();
        _mockPaymentMethodsService.Setup(x => x.GetAllPaymentMethodsAsync(_cancellationToken))
            .ReturnsAsync(fakePaymentMethods);

        // Act
        var result = await _controller.GetPaymentMethods(_cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(fakePaymentMethods, okResult.Value);
    }

    [Fact]
    public async Task GetCart_ReturnsOkObjectResult_WithOrder()
    {
        // Arrange
        var expectedOrder = new List<OrderGameDto>() { new() };
        _mockOrderService.Setup(s => s.GetUserOpenOrderAsync(It.IsAny<Guid>(), _cancellationToken))
            .ReturnsAsync(expectedOrder);

        // Act
        var result = await _controller.GetCart(_cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedOrder = Assert.IsType<List<OrderGameDto>>(okResult.Value);
        Assert.Equal(expectedOrder, returnedOrder);
    }

    [Fact]
    public async Task PayForOrder_ReturnsResultFromHandler()
    {
        // Arrange
        var paymentRequest = new PaymentRequest { Method = "CreditCard" };
        var mockPaymentResultHandler = new Mock<IPaymentResult>();

        _mockOrderService.Setup(s => s.PayForOrderAsync(It.IsAny<Guid>(), paymentRequest, _cancellationToken))
            .ReturnsAsync(mockPaymentResultHandler.Object);
        _mockPaymentResultHandler.Setup(s => s.HandleResult(mockPaymentResultHandler.Object))
            .Returns(new OkResult());

        // Act
        var result = await _controller.PayForOrder(paymentRequest, _cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task GetOrderDetails_ReturnsOkResult_WithOrderDetails()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var expectedOrderDetails = new List<OrderGameDto> { new() };
        _mockOrderService.Setup(x => x.GetOrderDetailsByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedOrderDetails);

        // Act
        var result = await _controller.GetOrderDetails(orderId, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var orderDetails = okResult.Value.Should().BeAssignableTo<List<OrderGameDto>>().Subject;
        orderDetails.Should().BeEquivalentTo(expectedOrderDetails);
    }
}
