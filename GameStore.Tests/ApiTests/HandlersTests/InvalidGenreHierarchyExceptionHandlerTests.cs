using GameStore.Common.Exceptions;
using GameStore.WebApi.Filters.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameStore.Tests.ApiTests.HandlersTests;

public class InvalidGenreHierarchyExceptionHandlerTests
{
    private readonly Mock<ILogger> _loggerMock = new();

    [Fact]
    public async Task BusinessRuleViolationExceptionHandler_HandleExceptionAsync_ShouldReturnUnprocessableEntity()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var exception = new InvalidGenreHierarchyException();
        var handler = new InvalidGenreHierarchyExceptionHandler();

        // Act
        await handler.HandleExceptionAsync(context, exception, _loggerMock.Object, CancellationToken.None);

        // Assert
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, context.Response.StatusCode);
    }

    [Fact]
    public void BusinessRuleViolationExceptionHandler_CanHandle_ShouldReturnTrue()
    {
        // Arrange
        var exception = new InvalidGenreHierarchyException();
        var handler = new InvalidGenreHierarchyExceptionHandler();

        // Act
        var result = handler.CanHandle(exception);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void BusinessRuleViolationExceptionHandler_CanHandle_ShouldReturnFalse()
    {
        // Arrange
        var exception = new Exception("Test exception");
        var handler = new InvalidGenreHierarchyExceptionHandler();

        // Act
        var result = handler.CanHandle(exception);

        // Assert
        Assert.False(result);
    }
}