using GameStore.BLL.DTOs.Payment;
using GameStore.BLL.Interfaces.PaymentServices;
using GameStore.BLL.Services.PaymentServices;
using GameStore.Domain.Entities.CoreEntities;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.PaymentServices;

public class PaymentServiceTests
{
    [Fact]
    public async Task PayForOrderAsync_CallsCorrectPaymentHandler_AndReturnsResult()
    {
        // Arrange
        var mockPaymentHandlerFactory = new Mock<IPaymentHandlerFactory>();
        var mockPaymentHandler = new Mock<IPaymentHandler>();
        var paymentMethod = "Visa";
        var order = new Order();
        var paymentRequest = new PaymentRequest { Method = paymentMethod };
        var paymentResultHandler = new Mock<IPaymentResult>().Object;
        var cancellationToken = new CancellationToken(false);

        mockPaymentHandlerFactory.Setup(f => f.GetPaymentHandler(paymentMethod))
            .Returns(mockPaymentHandler.Object);

        mockPaymentHandler.Setup(h => h.PayAsync(order, paymentRequest, cancellationToken))
            .ReturnsAsync(paymentResultHandler);

        var paymentService = new PaymentService(mockPaymentHandlerFactory.Object);

        // Act
        var result = await paymentService.PayForOrderAsync(order, paymentRequest, cancellationToken);

        // Assert
        Assert.Equal(paymentResultHandler, result);
        mockPaymentHandlerFactory.Verify(f => f.GetPaymentHandler(paymentMethod), Times.Once);
        mockPaymentHandler.Verify(h => h.PayAsync(order, paymentRequest, cancellationToken), Times.Once);
    }
}