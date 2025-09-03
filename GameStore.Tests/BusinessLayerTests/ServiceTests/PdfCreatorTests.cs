using GameStore.BLL.Interfaces;
using GameStore.BLL.Services.InvoiceCreator;
using GameStore.Domain.Entities.CoreEntities;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests;

public class PdfCreatorTests
{
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new();

    [Fact]
    public void GenerateInvoicePdf_Returns_ExpectedByteArray()
    {
        // Arrange
        var pdfCreator = new PdfCreator(_dateTimeProviderMock.Object);
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            Items =
            [
                new()
                {
                    Price = 10,
                    Quantity = 3,
                },

                new()
                {
                    Price = 20,
                    Quantity = 2,
                }

            ],
        };

        // Act
        var result = pdfCreator.GenerateInvoicePdf(order, 30);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }
}