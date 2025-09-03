using GameStore.WebApi.Filters.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameStore.Tests.ApiTests.HandlersTests;

public class KeyNotFoundExceptionHandlerTests
{
    private readonly Mock<ILogger> _loggerMock = new();

    [Fact]
    public async Task KeyNotFoundExceptionHandler_HandleExceptionAsync_ShouldReturnNotFound()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var exception = new KeyNotFoundException("Test exception");
        var handler = new KeyNotFoundExceptionHandler();

        // Act
        await handler.HandleExceptionAsync(context, exception, _loggerMock.Object, CancellationToken.None);

        // Assert
        Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
    }

    [Fact]
    public void KeyNotFoundExceptionHandler_CanHandle_ShouldReturnTrue()
    {
        // Arrange
        var exception = new KeyNotFoundException("Test exception");
        var handler = new KeyNotFoundExceptionHandler();

        // Act
        var result = handler.CanHandle(exception);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void KeyNotFoundExceptionHandler_CanHandle_ShouldReturnFalse()
    {
        // Arrange
        var exception = new Exception("Test exception");
        var handler = new KeyNotFoundExceptionHandler();

        // Act
        var result = handler.CanHandle(exception);

        // Assert
        Assert.False(result);
    }
}