using GameStore.BLL.Interfaces.PaymentServices;
using GameStore.BLL.Services.PaymentServices.ResultHandlers;
using GameStore.WebApi.PaymentResultHandler.Strategies;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Tests.ApiTests.PaymentResultHandlers;

public class InvoiceResultStrategyTests
{
    [Fact]
    public void CanHandle_WithInvoiceResult_ReturnsTrue()
    {
        // Arrange
        var invoiceResultMock = new InvoiceResult(new byte[15]);
        var strategy = new InvoiceResultStrategy();

        // Act
        bool canHandle = strategy.CanHandle(invoiceResultMock);

        // Assert
        Assert.True(canHandle);
    }

    [Fact]
    public void CanHandle_WithNonInvoiceResult_ReturnsFalse()
    {
        // Arrange
        var nonInvoiceResultMock = new Mock<IPaymentResult>();
        var strategy = new InvoiceResultStrategy();

        // Act
        bool canHandle = strategy.CanHandle(nonInvoiceResultMock.Object);

        // Assert
        Assert.False(canHandle);
    }

    [Fact]
    public void Handle_WithInvoiceResult_ReturnsFileContentResult()
    {
        // Arrange
        var expectedContent = new byte[] { 0x01, 0x02, 0x03 }; // Sample PDF content
        var invoiceResultMock = new InvoiceResult(expectedContent);

        var strategy = new InvoiceResultStrategy();

        // Act
        IActionResult actionResult = strategy.Handle(invoiceResultMock);

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(actionResult);
        Assert.Equal("application/pdf", fileResult.ContentType);
        Assert.Equal(expectedContent, fileResult.FileContents);
    }
}