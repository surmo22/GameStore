using GameStore.BLL.Interfaces.PaymentServices;
using GameStore.WebApi.PaymentResultHandler;
using GameStore.WebApi.PaymentResultHandler.Strategies;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Tests.ApiTests.PaymentResultHandlers;

public class PaymentResultHandlerTests
{
    [Fact]
    public void HandleResult_WithHandledPaymentResult_ReturnsStrategyResult()
    {
        // Arrange
        var paymentResultMock = new Mock<IPaymentResult>();
        var strategyMock = new Mock<IPaymentResultStrategy>();
        strategyMock.Setup(s => s.CanHandle(It.IsAny<IPaymentResult>())).Returns(true);

        IActionResult expectedAction = new OkResult();
        strategyMock.Setup(s => s.Handle(It.IsAny<IPaymentResult>())).Returns(expectedAction);

        var strategies = new List<IPaymentResultStrategy> { strategyMock.Object };
        var handler = new PaymentResultHandler(strategies);

        // Act
        var result = handler.HandleResult(paymentResultMock.Object);

        // Assert
        Assert.Equal(expectedAction, result);
        strategyMock.Verify(s => s.Handle(It.IsAny<IPaymentResult>()), Times.Once());
    }

    [Fact]
    public void HandleResult_WithUnhandledPaymentResult_ReturnsInternalServerError()
    {
        // Arrange
        var paymentResultMock = new Mock<IPaymentResult>();
        var handler = new PaymentResultHandler(new List<IPaymentResultStrategy>());

        // Act
        var result = handler.HandleResult(paymentResultMock.Object);

        // Assert
        Assert.IsType<StatusCodeResult>(result);
        var statusCodeResult = result as StatusCodeResult;
        Assert.Equal(500, statusCodeResult.StatusCode);
    }
}