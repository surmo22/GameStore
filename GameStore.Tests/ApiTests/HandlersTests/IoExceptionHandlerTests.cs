using GameStore.WebApi.Filters.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameStore.Tests.ApiTests.HandlersTests;

public class IoExceptionHandlerTests
{
    private readonly Mock<ILogger> _loggerMock = new();

    [Fact]
    public void IoExceptionHandler_CanHandle_ShouldReturnTrue()
    {
        // Arrange
        var exception = new IOException("Test exception");
        var handler = new IoExceptionHandler();

        // Act
        var result = handler.CanHandle(exception);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IoExceptionHandler_CanHandle_ShouldReturnFalse()
    {
        // Arrange
        var exception = new Exception("Test exception");
        var handler = new IoExceptionHandler();

        // Act
        var result = handler.CanHandle(exception);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IoExceptionHandler_HandleExceptionAsync_ShouldReturnConflict()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var exception = new IOException("Test exception");
        var handler = new IoExceptionHandler();

        // Act
        await handler.HandleExceptionAsync(context, exception, _loggerMock.Object, CancellationToken.None);

        // Assert
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
    }
}