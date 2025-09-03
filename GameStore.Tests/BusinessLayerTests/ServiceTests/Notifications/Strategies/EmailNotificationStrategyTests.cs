using GameStore.BLL.DTOs.Notifications;
using GameStore.BLL.Interfaces.NotificationServices;
using GameStore.BLL.Services.NotificationServices.NotificationStrategies;
using GameStore.Domain.Entities.Enums;
using GameStore.Domain.Entities.UserEntities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.Notifications.Strategies;

public class EmailNotificationStrategyTests
{
    private readonly Mock<IAzureServiceBusService> _mockServiceBus = new();
    private readonly Mock<ILogger<EmailNotificationStrategy>> _mockLogger = new();
    private readonly EmailNotificationStrategy _strategy;

    public EmailNotificationStrategyTests()
    {
        _strategy = new EmailNotificationStrategy(_mockServiceBus.Object, _mockLogger.Object);
    }

    [Fact]
    public void IsEnabledForUser_ReturnsTrue_WhenUserHasEmailNotificationType()
    {
        var user = new User
        {
            UserNotificationTypes = new List<UserNotificationTypes> { UserNotificationTypes.Email }
        };

        var result = _strategy.IsEnabledForUser(user);

        Assert.True(result);
    }

    [Fact]
    public void IsEnabledForUser_ReturnsFalse_WhenUserDoesNotHaveEmailNotificationType()
    {
        var user = new User
        {
            UserNotificationTypes = new List<UserNotificationTypes>() // empty list
        };

        var result = _strategy.IsEnabledForUser(user);

        Assert.False(result);
    }

    [Fact]
    public async Task SendNotificationAsync_QueuesEmail_WhenUserHasEmail()
    {
        var user = new User
        {
            Email = "test@example.com"
        };
        var subject = "Test Subject";
        var message = "Test Message";

        await _strategy.SendNotificationAsync(user, subject, message, CancellationToken.None);

        _mockServiceBus.Verify(
            s => s.SendMessageAsync(
                It.Is<EmailPaylod>(p => 
                    p.To == user.Email && 
                    p.Subject == subject && 
                    p.Body == message),
                "emailqueue",
                CancellationToken.None),
            Times.Once);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Successfully queued")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task SendNotificationAsync_DoesNotQueueEmail_WhenUserEmailIsNullOrEmpty()
    {
        var userWithNullEmail = new User { Email = null };
        var userWithEmptyEmail = new User { Email = "" };

        await _strategy.SendNotificationAsync(userWithNullEmail, "subj", "msg", CancellationToken.None);
        await _strategy.SendNotificationAsync(userWithEmptyEmail, "subj", "msg", CancellationToken.None);

        _mockServiceBus.Verify(s => s.SendMessageAsync(It.IsAny<EmailPaylod>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Email notification was not sent")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Exactly(2));
    }
}