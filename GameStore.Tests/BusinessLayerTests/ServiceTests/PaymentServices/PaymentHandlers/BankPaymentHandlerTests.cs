using GameStore.BLL.DTOs.Payment;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Options;
using GameStore.BLL.Services.PaymentServices.PaymentHandlers;
using GameStore.BLL.Services.PaymentServices.ResultHandlers;
using GameStore.Domain.Entities.CoreEntities;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.PaymentServices.PaymentHandlers;

public class BankPaymentHandlerTests
{
    private readonly Mock<IPdfCreator> _pdfCreatorMock = new();
    private readonly Mock<IOptions<BankOptions>> _optionsMock = new();
    private readonly BankPaymentHandler _bankPaymentHandler;

    public BankPaymentHandlerTests()
    {
        _optionsMock.Setup(o => o.Value).Returns(new BankOptions { Title = "BankPayment", InvoiceDaysValid = 30 });
        _bankPaymentHandler = new BankPaymentHandler(_pdfCreatorMock.Object, _optionsMock.Object);
    }

    [Fact]
    public void CanHandle_ReturnsTrueForCorrectHandlerName()
    {
        // Act
        bool result = _bankPaymentHandler.CanHandle("BankPayment");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanHandle_ReturnsFalseForIncorrectHandlerName()
    {
        // Act
        bool result = _bankPaymentHandler.CanHandle("OtherPaymentHandler");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task PayAsync_GeneratesInvoiceSuccessfully()
    {
        // Arrange
        var order = new Order();
        var paymentRequest = new PaymentRequest();
        var expectedInvoicePdf = new byte[] { 1, 2 };
        _pdfCreatorMock.Setup(p => p.GenerateInvoicePdf(order, 30))
            .Returns(expectedInvoicePdf);

        // Act
        var result = await _bankPaymentHandler.PayAsync(order, paymentRequest, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        _pdfCreatorMock.Verify(p => p.GenerateInvoicePdf(order, 30), Times.Once);
        Assert.IsType<InvoiceResult>(result);
    }
}