using GameStore.BLL.Interfaces;
using GameStore.BLL.Interfaces.Repositories;
using GameStore.BLL.Services.OrderServices;
using GameStore.Common.Constants;
using GameStore.Domain.Entities.CoreEntities;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.OrderServices;

public class OrderManagerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGuidProvider> _guidProvider = new();
    private readonly OrderManager _orderManager;

    public OrderManagerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _guidProvider.Setup(g => g.NewGuid()).Returns(Guid.NewGuid);
        _orderManager = new OrderManager(_guidProvider.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetOrCreateOpenOrderAsync_ReturnsExistingOrder()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var existingOrder = new Order { Id = Guid.NewGuid(), Status = OrderStatuses.Open };

        _unitOfWorkMock.Setup(x => x.Orders.GetCustomersOpenOrderAsync(customerId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(existingOrder);

        // Act
        var result = await _orderManager.GetOrCreateOpenOrderAsync(customerId, CancellationToken.None);

        // Assert
        Assert.Equal(existingOrder.Id, result.Id);
    }

    [Fact]
    public async Task GetOrCreateOpenOrderAsync_CreatesNewOrder_WhenNoExistingOrder()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        _unitOfWorkMock.Setup(x => x.Orders.GetCustomersOpenOrderAsync(customerId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((Order)null!);
        _unitOfWorkMock.Setup(x => x.Orders.AddAsync(It.IsAny<Order>())).Verifiable();
        _unitOfWorkMock.Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>())).Verifiable();

        // Act
        var result = await _orderManager.GetOrCreateOpenOrderAsync(customerId, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(x => x.Orders.AddAsync(It.IsAny<Order>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CompleteAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(customerId, result.CustomerId);
        Assert.Equal(OrderStatuses.Open, result.Status);
    }

    [Fact]
    public async Task DeleteOrderIfEmptyAsync_DeletesOrder_WhenEmpty()
    {
        // Arrange
        var order = new Order { Items = [] };

        _unitOfWorkMock.Setup(x => x.Orders.DeleteAsync(order.Id, It.IsAny<CancellationToken>())).Verifiable();
        _unitOfWorkMock.Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>())).Verifiable();

        // Act
        await _orderManager.DeleteOrderIfEmptyAsync(order, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(x => x.Orders.DeleteAsync(order.Id, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CompleteAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteOrderIfEmptyAsync_DoesNotDeleteOrder_WhenNotEmpty()
    {
        // Arrange
        var order = new Order { Items = [new()] };

        _unitOfWorkMock.Setup(x => x.Orders.DeleteAsync(order.Id, It.IsAny<CancellationToken>())).Verifiable();

        // Act
        await _orderManager.DeleteOrderIfEmptyAsync(order, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(x => x.Orders.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CompleteAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}