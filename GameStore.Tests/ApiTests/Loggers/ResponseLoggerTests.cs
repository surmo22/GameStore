using GameStore.WebApi.Middlewares.Loggers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameStore.Tests.ApiTests.Loggers;

public class ResponseLoggerTests
{
    private readonly Mock<ILogger<ResponseLogger>> _loggerMock;
    private readonly ResponseLogger _responseLogger;

    public ResponseLoggerTests()
    {
        _loggerMock = new Mock<ILogger<ResponseLogger>>();
        _responseLogger = new ResponseLogger(_loggerMock.Object);
    }

    [Fact]
    public void LogResponse_WithContent_LogsCorrectly()
    {
        // Arrange
        // Act
        _responseLogger.LogResponse("url",200, []);

        // Assert
        _loggerMock.Verify(
            l => l.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Once);
    }
}