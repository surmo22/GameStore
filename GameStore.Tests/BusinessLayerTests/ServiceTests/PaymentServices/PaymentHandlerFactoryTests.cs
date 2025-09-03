using GameStore.BLL.Interfaces.PaymentServices;
using GameStore.BLL.Services.PaymentServices;
using GameStore.Common.Exceptions;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.PaymentServices;

public class PaymentHandlerFactoryTests
{
    [Fact]
    public void GetPaymentHandler_ReturnsCorrectHandler_WhenSupported()
    {
        // Arrange
        var mockHandler1 = new Mock<IPaymentHandler>();
        var mockHandler2 = new Mock<IPaymentHandler>();
        mockHandler1.Setup(x => x.CanHandle("Visa")).Returns(false);
        mockHandler2.Setup(x => x.CanHandle("Visa")).Returns(true);

        var paymentHandlers = new List<IPaymentHandler> { mockHandler1.Object, mockHandler2.Object };

        var factory = new PaymentHandlerFactory(paymentHandlers);

        // Act
        var handler = factory.GetPaymentHandler("Visa");

        // Assert
        Assert.Equal(mockHandler2.Object, handler);
    }

    [Fact]
    public void GetPaymentHandler_ThrowsException_WhenNotSupported()
    {
        // Arrange
        var mockHandler1 = new Mock<IPaymentHandler>();
        var mockHandler2 = new Mock<IPaymentHandler>();
        mockHandler1.Setup(x => x.CanHandle(It.IsAny<string>())).Returns(false);
        mockHandler2.Setup(x => x.CanHandle(It.IsAny<string>())).Returns(false);

        var paymentHandlers = new List<IPaymentHandler> { mockHandler1.Object, mockHandler2.Object };

        var factory = new PaymentHandlerFactory(paymentHandlers);

        // Act & Assert
        var exception = Assert.Throws<PaymentMethodIsNotSupportedException>(() => factory.GetPaymentHandler("Bitcoin"));
        Assert.Contains("Bitcoin", exception.Message);
    }
}