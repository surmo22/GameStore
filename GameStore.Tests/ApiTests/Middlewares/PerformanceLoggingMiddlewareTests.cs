using GameStore.WebApi.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameStore.Tests.ApiTests.Middlewares;

public class PerformanceLoggingMiddlewareTests
{
    private readonly PerformanceLoggingMiddleware _testClass;
    private readonly Mock<ILogger<PerformanceLoggingMiddleware>> _logger = new();

    public PerformanceLoggingMiddlewareTests()
    {
        _testClass = new PerformanceLoggingMiddleware(async (_) => await Task.Delay(50), _logger.Object);
    }

    [Fact]
    public async Task InvokeAsync_LogsExecutionTime()
    {
        // Arrange
        var context = new DefaultHttpContext();

        // Act
        await _testClass.InvokeAsync(context);

        // Assert
        _logger.Verify(
            l => l.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.AtLeastOnce);
    }
}