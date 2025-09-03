using GameStore.BLL.Interfaces.PaymentServices;
using GameStore.BLL.Services.PaymentServices.ResultHandlers;
using GameStore.WebApi.PaymentResultHandler.Strategies;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Tests.ApiTests.PaymentResultHandlers;

public class VisaResultStrategyTests
{
    [Fact]
    public void CanHandle_WithVisaResult_ReturnsTrue()
    {
        // Arrange
        var visaResultMock = new Mock<VisaResult>();
        var strategy = new VisaResultStrategy();

        // Act
        bool canHandle = strategy.CanHandle(visaResultMock.Object);

        // Assert
        Assert.True(canHandle);
    }

    [Fact]
    public void CanHandle_WithNonVisaResult_ReturnsFalse()
    {
        // Arrange
        var nonVisaResultMock = new Mock<IPaymentResult>();
        var strategy = new VisaResultStrategy();

        // Act
        bool canHandle = strategy.CanHandle(nonVisaResultMock.Object);

        // Assert
        Assert.False(canHandle);
    }

    [Fact]
    public void Handle_WithVisaResult_ReturnsOkResult()
    {
        // Arrange
        var visaResultMock = new Mock<VisaResult>();
        var strategy = new VisaResultStrategy();

        // Act
        IActionResult result = strategy.Handle(visaResultMock.Object);

        // Assert
        Assert.IsType<OkResult>(result);
    }
}