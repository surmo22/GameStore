using System.Net;
using System.Text;
using GameStore.WebApi.Middlewares.Loggers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameStore.Tests.ApiTests.Loggers;

public class RequestLoggerTests
{
    private readonly Mock<ILogger<RequestLogger>> _loggerMock;
    private readonly RequestLogger _requestLogger;

    public RequestLoggerTests()
    {
        _loggerMock = new Mock<ILogger<RequestLogger>>();
        _requestLogger = new RequestLogger(_loggerMock.Object);
    }

    [Fact]
    public async Task LogRequestAsync_WithContent_LogsCorrectly()
    {
        // Arrange
        var context = GetHttpContextMock("Test request content", "POST");

        // Act
        await _requestLogger.LogRequestAsync(context);

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

    private static DefaultHttpContext GetHttpContextMock(string requestContent, string httpMethod)
    {
        var context = new DefaultHttpContext
        {
            Connection =
            {
                RemoteIpAddress = IPAddress.Parse("127.0.0.1"),
            },
            Request =
            {
                Path = "/test-url",
                Method = httpMethod,
            },
        };

        if (!string.IsNullOrEmpty(requestContent))
        {
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(requestContent));
        }

        return context;
    }
}