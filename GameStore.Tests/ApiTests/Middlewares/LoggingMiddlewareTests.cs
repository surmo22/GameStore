using GameStore.WebApi.Middlewares;
using GameStore.WebApi.Middlewares.Loggers;
using Microsoft.AspNetCore.Http;
using Microsoft.IO;
using Moq;
using Xunit;

namespace GameStore.Tests.ApiTests.Middlewares;

public class LoggingMiddlewareTests
{
    private readonly Mock<IRequestLogger> _requestLoggerMock;
    private readonly Mock<RequestDelegate> _nextDelegateMock;
    private readonly LoggingMiddleware _middleware;

    public LoggingMiddlewareTests()
    {
        _nextDelegateMock = new Mock<RequestDelegate>();
        _requestLoggerMock = new Mock<IRequestLogger>();
        var responseLoggerMock = new Mock<IResponseLogger>();
        var recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();

        _middleware = new LoggingMiddleware(
            _nextDelegateMock.Object,
            _requestLoggerMock.Object,
            responseLoggerMock.Object,
            recyclableMemoryStreamManager);
    }

    [Fact]
    public async Task InvokeAsync_CallsLoggersAndCopiesStream()
    {
        // Arrange
        var expectedResponseText = "Hello, test response!";
        var originalBodyStream = new MemoryStream();

        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = originalBodyStream
            }
        };

        _nextDelegateMock
            .Setup(next => next.Invoke(context))
            .Returns(async () =>
            {
                await context.Response.WriteAsync(expectedResponseText);
            });

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        _requestLoggerMock.Verify(x => x.LogRequestAsync(context), Times.Once);
        _nextDelegateMock.Verify(x => x.Invoke(context), Times.Once);

        // Reset position to read what was written
        originalBodyStream.Position = 0;
        using var reader = new StreamReader(originalBodyStream);
        var actualResponseText = await reader.ReadToEndAsync();

        Assert.Equal(expectedResponseText, actualResponseText);
    }
}