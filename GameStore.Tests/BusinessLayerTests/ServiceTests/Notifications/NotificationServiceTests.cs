using GameStore.BLL.Interfaces.NotificationServices;
using GameStore.BLL.Interfaces.Security;
using GameStore.BLL.Services.NotificationServices;
using GameStore.Common.Constants;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.Entities.Enums;
using GameStore.Domain.Entities.UserEntities;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.Notifications;

public class NotificationServiceTests
{
    private readonly Mock<ICurrentUserService> _mockCurrentUserService = new();
    private readonly Mock<INotificationUserService> _mockNotificationUserService = new();
    private readonly Mock<INotificationStrategy> _mockStrategy1 = new();
    private readonly Mock<INotificationStrategy> _mockStrategy2 = new();

    private readonly NotificationService _service;

    public NotificationServiceTests()
    {

        var strategies = new List<INotificationStrategy> { _mockStrategy1.Object, _mockStrategy2.Object };

        _service = new NotificationService(
            _mockCurrentUserService.Object,
            _mockNotificationUserService.Object,
            strategies);
    }

    [Fact]
    public async Task GetUserNotificationTypes_ReturnsUserNotificationTypes()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var notificationTypes = new List<UserNotificationTypes> { UserNotificationTypes.Email, UserNotificationTypes.Sms };

        _mockCurrentUserService.Setup(s => s.UserId).Returns(userId);
        _mockNotificationUserService.Setup(n => n
                .GetUserNotificationTypesAsync(userId, CancellationToken.None))
            .ReturnsAsync(notificationTypes);

        // Act
        var result = await _service.GetUserNotificationTypes(CancellationToken.None);

        // Assert
        Assert.Equal(notificationTypes, result);
    }

    [Fact]
    public async Task UpdateUserNotificationTypes_UpdatesUserNotificationTypesAndCallsUpdate()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var newTypes = new List<UserNotificationTypes> { UserNotificationTypes.Push };

        _mockCurrentUserService.Setup(s => s.UserId).Returns(userId);
        _mockNotificationUserService.Setup(n =>
            n.UpdateUserNotificationTypesAsync(userId, newTypes, CancellationToken.None));

        // Act
        await _service.UpdateUserNotificationTypes(newTypes, CancellationToken.None);

        // Assert
        _mockCurrentUserService.Verify(n => n.UserId, Times.Once);
    }

    [Fact]
    public async Task SendNotificationAboutOrderStatusToRelatedUsers_SendsNotificationsToUsers()
    {
        // Arrange
        var order = new Order { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid(), Status = OrderStatuses.Shipped };

        var managerUser = new User { Id = Guid.NewGuid(), UserName = "manager1" };
        var orderOwner = new User { Id = order.CustomerId, UserName = "owner" };

        var relatedUsers = new List<User> { managerUser };
        _mockNotificationUserService.Setup(n =>
            n.GetUsersByClaimAsync(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(relatedUsers);
        _mockNotificationUserService.Setup(n => n.GetUserByIdAsync(order.CustomerId, CancellationToken.None))
            .ReturnsAsync(orderOwner);
        _mockStrategy1.Setup(s => s.IsEnabledForUser(It.IsAny<User>())).Returns(true);
        _mockStrategy1.Setup(s => s.SendNotificationAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockStrategy2.Setup(s => s.IsEnabledForUser(It.IsAny<User>())).Returns(false);

        // Act
        await _service.SendNotificationAboutOrderStatusToRelatedUsers(order, CancellationToken.None);

        // Assert
        _mockStrategy1.Verify(s => s.SendNotificationAsync(managerUser, "Order status changed",
            $"Order {order.Id} has changed its status to {order.Status}", CancellationToken.None), Times.Once);

        _mockStrategy1.Verify(s => s.SendNotificationAsync(orderOwner, "Order status changed",
            $"Order {order.Id} has changed its status to {order.Status}", CancellationToken.None), Times.Once);

        _mockStrategy2.Verify(s => s.SendNotificationAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}