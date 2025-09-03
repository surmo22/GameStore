using AutoMapper;
using GameStore.BLL.DTOs.Payment;
using GameStore.BLL.Interfaces.ExternalServices;
using GameStore.BLL.Options;
using GameStore.BLL.Services.ExternalServices.Requests;
using GameStore.BLL.Services.ExternalServices.Responses;
using GameStore.BLL.Services.PaymentServices.PaymentHandlers;
using GameStore.BLL.Services.PaymentServices.ResultHandlers;
using GameStore.Domain.Entities.CoreEntities;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.PaymentServices.PaymentHandlers;

public class BoxPaymentHandlerTests
{
    private readonly Mock<IPaymentExternalService> _restClientMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IOptions<BoxPaymentOptions>> _optionsMock = new();
    private readonly BoxPaymentHandler _iBoxPaymentHandler;

    public BoxPaymentHandlerTests()
    {
        _optionsMock.Setup(o => o.Value).Returns(new BoxPaymentOptions { Title = "IBoxPayment", RelativePath = "test", Endpoint = "https://api.ibox.com/pay" });

        _iBoxPaymentHandler = new BoxPaymentHandler(_restClientMock.Object, _mapperMock.Object, _optionsMock.Object);
    }

    [Fact]
    public void CanHandle_ReturnsTrueForCorrectHandlerName()
    {
        // Act
        bool result = _iBoxPaymentHandler.CanHandle("IBoxPayment");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanHandle_ReturnsFalseForIncorrectHandlerName()
    {
        // Act
        bool result = _iBoxPaymentHandler.CanHandle("OtherPaymentHandler");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task PayAsync_SuccessfullyProcessesPayment()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            Items = [new()
                {
                    Price = 100,
                    Quantity = 1,
                },
            ],
        };
        var paymentRequest = new PaymentRequest();
        var iBoxResponse = new BoxPaymentResponse();
        var iBoxPaymentDto = new BoxPaymentDto();

        _restClientMock.Setup(x => x.PayWithBox(
                It.IsAny<string>(), It.IsAny<BoxPaymentRequest>()))
            .ReturnsAsync(iBoxResponse);

        _mapperMock.Setup(m => m.Map<BoxPaymentDto>(iBoxResponse)).Returns(iBoxPaymentDto);

        // Act
        var result = await _iBoxPaymentHandler.PayAsync(order, paymentRequest, CancellationToken.None);

        // Assert
        _mapperMock.Verify(m => m.Map<BoxPaymentDto>(iBoxResponse), Times.Once);
        Assert.NotNull(result);
        Assert.IsType<IBoxResult>(result);
        _restClientMock.Verify(x => x.PayWithBox(_optionsMock.Object.Value.Endpoint, It.IsAny<BoxPaymentRequest>()), Times.Once);
    }
}