using GameStore.Common.Exceptions;
using GameStore.WebApi.Filters.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameStore.Tests.ApiTests.HandlersTests;

public class PaymentMethodIsNotSupportedExceptionHandlerTests
{
    private readonly Mock<ILogger> _loggerMock = new();

    [Fact]
    public async Task PaymentMethodIsNotSupportedExceptionHandler_HandleExceptionAsync_ShouldReturnBadRequest()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var exception = new PaymentMethodIsNotSupportedException("Test exception");
        var handler = new PaymentMethodIsNotSupportedExceptionHandler();

        // Act
        await handler.HandleExceptionAsync(context, exception, _loggerMock.Object, CancellationToken.None);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
    }

    [Fact]
    public void PaymentMethodIsNotSupportedExceptionHandler_CanHandle_ShouldReturnTrue()
    {
        // Arrange
        var exception = new PaymentMethodIsNotSupportedException("Test exception");
        var handler = new PaymentMethodIsNotSupportedExceptionHandler();

        // Act
        var result = handler.CanHandle(exception);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void PaymentMethodIsNotSupportedExceptionHandler_CanHandle_ShouldReturnFalse()
    {
        // Arrange
        var exception = new Exception("Test exception");
        var handler = new PaymentMethodIsNotSupportedExceptionHandler();

        // Act
        var result = handler.CanHandle(exception);

        // Assert
        Assert.False(result);
    }
}