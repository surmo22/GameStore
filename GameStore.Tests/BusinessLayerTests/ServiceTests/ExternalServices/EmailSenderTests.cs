using System.Net;
using GameStore.BLL.DTOs.Notifications;
using GameStore.BLL.Options;
using GameStore.BLL.Services.ExternalServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SendGrid;
using SendGrid.Helpers.Mail;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.ExternalServices;

public class SendGridEmailSenderTests
{
    private readonly Mock<ISendGridClient> _mockClient = new();
    private readonly Mock<ILogger<SendGridEmailSender>> _mockLogger = new();
    private readonly IOptions<SenderOptions> _options;

    public SendGridEmailSenderTests()
    {
        _options = Options.Create(new SenderOptions
        {
            SenderEmail = "sender@example.com",
            SenderName = "Sender Name"
        });
    }

    [Fact]
    public async Task SendEmailAsync_SuccessfulSend_LogsInformation()
    {
        // Arrange
        var emailPayload = new EmailPaylod
        {
            To = "recipient@example.com",
            Subject = "Test Subject",
            Body = "Test Body"
        };

        var response = new Response(HttpStatusCode.Accepted, null, null);
        _mockClient.Setup(c => c.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var service = new SendGridEmailSender(_mockClient.Object, _options, _mockLogger.Object);

        // Act
        await service.SendEmailAsync(emailPayload);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Email sent")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _mockClient.Verify(c => c.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendEmailAsync_FailedSend_ThrowsInvalidOperationExceptionAndLogsError()
    {
        // Arrange
        var emailPayload = new EmailPaylod
        {
            To = "recipient@example.com",
            Subject = "Test Subject",
            Body = "Test Body"
        };

        var response = new Response(HttpStatusCode.BadRequest, Mock.Of<HttpContent>(), null);

        _mockClient.Setup(c => c.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var service = new SendGridEmailSender(_mockClient.Object, _options, _mockLogger.Object);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.SendEmailAsync(emailPayload));

        Assert.Equal("Failed to send email.", ex.Message);
    }
}