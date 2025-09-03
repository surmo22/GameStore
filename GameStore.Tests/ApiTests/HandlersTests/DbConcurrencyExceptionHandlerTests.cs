using System.Data;
using GameStore.WebApi.Filters.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameStore.Tests.ApiTests.HandlersTests;

public class DbConcurrencyExceptionHandlerTests
{
    private readonly Mock<ILogger> _loggerMock = new();

    [Fact]
    public async Task DbConcurrencyExceptionHandler_HandleExceptionAsync_ShouldReturnConflict()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var exception = new DBConcurrencyException("Test exception");
        var handler = new DbConcurrencyExceptionHandler();

        // Act
        await handler.HandleExceptionAsync(context, exception, _loggerMock.Object, CancellationToken.None);

        // Assert
        Assert.Equal(StatusCodes.Status409Conflict, context.Response.StatusCode);
    }

    [Fact]
    public void DbConcurrencyExceptionHandler_CanHandle_ShouldReturnTrue()
    {
        // Arrange
        var exception = new DBConcurrencyException("Test exception");
        var handler = new DbConcurrencyExceptionHandler();

        // Act
        var result = handler.CanHandle(exception);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void DbConcurrencyExceptionHandler_CanHandle_ShouldReturnFalse()
    {
        // Arrange
        var exception = new Exception("Test exception");
        var handler = new DbConcurrencyExceptionHandler();

        // Act
        var result = handler.CanHandle(exception);

        // Assert
        Assert.False(result);
    }
}