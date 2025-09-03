using AutoMapper;
using GameStore.BLL.DTOs.PaymentMethods;
using GameStore.BLL.Interfaces.Repositories;
using GameStore.BLL.Services.PaymentServices;
using GameStore.Domain.Entities.CoreEntities;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.PaymentServices;

public class PaymentMethodsServiceTests
{
    [Fact]
    public async Task GetAllPaymentMethodsAsync_ReturnsMappedPaymentMethods()
    {
        // Arrange
        var mockRepository = new Mock<IPaymentMethodsRepository>();
        var mockMapper = new Mock<IMapper>();

        var paymentMethodEntities = new List<PaymentMethod>
        {
            new() { Title = "Visa" },
            new() { Title = "MasterCard" },
        };

        var paymentMethodDtos = new List<PaymentMethodDto>
        {
            new() { Title = "Visa" },
            new() { Title = "MasterCard" },
        };

        mockRepository.Setup(r => r.GetAllPaymentMethodsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentMethodEntities);

        mockMapper.Setup(m => m.Map<PaymentMethodDto>(It.IsAny<PaymentMethod>()))
            .Returns((PaymentMethod source) => paymentMethodDtos.FirstOrDefault(dto => dto.Title == source.Title));

        var service = new PaymentMethodsService(mockRepository.Object, mockMapper.Object);

        // Act
        var result = await service.GetAllPaymentMethodsAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(paymentMethodDtos.Count, result.PaymentMethods.Count);
        for (var i = 0; i < paymentMethodDtos.Count; i++)
        {
            Assert.Equal(paymentMethodDtos[i].Title, result.PaymentMethods[i].Title);
        }
    }
}