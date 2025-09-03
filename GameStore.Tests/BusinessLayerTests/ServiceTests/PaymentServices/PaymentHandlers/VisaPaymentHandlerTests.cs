using GameStore.BLL.DTOs.Payment;
using GameStore.BLL.Interfaces.ExternalServices;
using GameStore.BLL.Options;
using GameStore.BLL.Services.ExternalServices.Requests;
using GameStore.BLL.Services.PaymentServices.PaymentHandlers;
using GameStore.Domain.Entities.CoreEntities;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.PaymentServices.PaymentHandlers;

public class VisaPaymentHandlerTests
{
    [Fact]
    public async Task PayAsync_SendsCorrectDataToRestClient()
    {
        // Arrange
        var mockRestClient = new Mock<IPaymentExternalService>();
        var options = Options.Create(new VisaPaymentOptions
        {
            Title = "Visa",
            Endpoint = "https://test.com",
        });
        var handler = new VisaPaymentHandler(mockRestClient.Object, options);
        var order = new Order { Items = [new() { Quantity = 1, Price = 100, Discount = 0 }] };
        var paymentRequest = new PaymentRequest
        {
            Visa = new VisaPaymentData()
            {
                CardNumber = "1234567890123456",
                Holder = "John Doe",
                Cvv2 = 123,
                MonthExpire = 12,
                YearExpire = 2026,
            },
        };
        var cancellationToken = new CancellationToken(false);

        // Act
        await handler.PayAsync(order, paymentRequest, cancellationToken);

        // Assert
        mockRestClient.Verify(
            x => x.PayWithVisa(
            "https://test.com",
            It.Is<VisaPaymentRequest>(vpr =>
                vpr.CardNumber == "1234567890123456" &&
                vpr.CardHolderName == "John Doe" &&
                vpr.Cvv == 123 &&
                vpr.ExpirationMonth == 12 &&
                vpr.ExpirationYear == 2026)));
    }

    [Theory]
    [InlineData("Visa", true)]
    [InlineData("MasterCard", false)]
    [InlineData("visa", false)]
    [InlineData("VISA", false)]
    public void CanHandle_ReturnsCorrectlyBasedOnMethodName(string methodName, bool expectedResult)
    {
        // Arrange
        var mockRestClient = new Mock<IPaymentExternalService>();
        var options = new VisaPaymentOptions
        {
            Title = "Visa",
            RelativePath = "visa",
        };
        var optionsWrapper = new OptionsWrapper<VisaPaymentOptions>(options);
        var handler = new VisaPaymentHandler(mockRestClient.Object, optionsWrapper);

        // Act
        var result = handler.CanHandle(methodName);

        // Assert
        Assert.Equal(expectedResult, result);
    }
}