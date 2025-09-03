using GameStore.BLL.DTOs.Notifications;
using GameStore.BLL.Interfaces.ExternalServices;
using GameStore.EmailSender;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameStore.Tests.AzureServicesTests.AzureEmailSenderTests;

public class EmailSenderFunctionTests
{
    private readonly Mock<IEmailSender> _emailSenderMock = new();
    private readonly Mock<ILogger<EmailSenderFunction>> _loggerMock = new();
    private readonly EmailSenderFunction _function;

    public EmailSenderFunctionTests()
    {
        _function = new EmailSenderFunction(_emailSenderMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Run_Success_CallsSendEmailAndLogsInformation()
    {
        // Arrange
        var emailPayload = new EmailPaylod
        {
            To = "test@example.com",
            Subject = "Hello",
            Body = "Body content"
        };

        _emailSenderMock.Setup(es => es.SendEmailAsync(emailPayload, CancellationToken.None))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await _function.Run(emailPayload);

        // Assert
        _emailSenderMock.Verify();
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Function triggered to process a message")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Run_WhenSendEmailThrows_LogsError()
    {
        // Arrange
        var emailPayload = new EmailPaylod
        {
            To = "fail@example.com",
            Subject = "Fail",
            Body = "Body content"
        };

        var exception = new Exception("Send failure");
        _emailSenderMock.Setup(es => es.SendEmailAsync(emailPayload, CancellationToken.None))
            .ThrowsAsync(exception);

        // Act
        await _function.Run(emailPayload);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed to send email")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}