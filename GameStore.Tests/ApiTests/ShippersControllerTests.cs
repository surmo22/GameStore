using GameStore.BLL.DTOs;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.WebApi.Controllers;
using Moq;
using Xunit;

namespace GameStore.Tests.ApiTests;

public class ShippersControllerTests
{
    private readonly Mock<IShippersService> _shippersServiceMock;
    private readonly ShippersController _controller;

    public ShippersControllerTests()
    {
        _shippersServiceMock = new Mock<IShippersService>();
        _controller = new ShippersController(_shippersServiceMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsListOfShippers()
    {
        // Arrange
        var expectedShippers = new List<GetShipperDto>
        {
            new(),
            new(),
        };

        _shippersServiceMock.Setup(s => s.GetAllShippersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedShippers);

        // Act
        var result = await _controller.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.Equal(expectedShippers, result);
    }
}