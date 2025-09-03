using GameStore.BLL.DTOs.Payment;
using GameStore.BLL.Interfaces.PaymentServices;
using GameStore.BLL.Services.PaymentServices.ResultHandlers;
using GameStore.WebApi.PaymentResultHandler.Strategies;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Tests.ApiTests.PaymentResultHandlers;

public class BoxResultStrategyTests
{
    [Fact]
    public void CanHandle_WithBoxResult_ReturnsTrue()
    {
        // Arrange
        var boxResultMock = new IBoxResult(new BoxPaymentDto());
        var strategy = new BoxResultStrategy();

        // Act
        bool canHandle = strategy.CanHandle(boxResultMock);

        // Assert
        Assert.True(canHandle);
    }

    [Fact]
    public void CanHandle_WithNonBoxResult_ReturnsFalse()
    {
        // Arrange
        var nonBoxResultMock = new Mock<IPaymentResult>();
        var strategy = new BoxResultStrategy();

        // Act
        bool canHandle = strategy.CanHandle(nonBoxResultMock.Object);

        // Assert
        Assert.False(canHandle);
    }

    [Fact]
    public void Handle_WithBoxResult_ReturnsCorrectResponse()
    {
        // Arrange
        var boxResultMock = new IBoxResult(new BoxPaymentDto());

        var strategy = new BoxResultStrategy();

        // Act
        var actionResult = strategy.Handle(boxResultMock);

        // Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.NotNull(okObjectResult.Value);
    }
}