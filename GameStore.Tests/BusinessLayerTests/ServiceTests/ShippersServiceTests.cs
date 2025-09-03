using AutoMapper;
using GameStore.BLL.DTOs;
using GameStore.BLL.Interfaces.Repositories;
using GameStore.BLL.Services.EntityServices;
using GameStore.Domain.MongoEntities;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests;

public class ShippersServiceTests
{
    private readonly Mock<IShippersRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ShippersService _service;

    public ShippersServiceTests()
    {
        _repositoryMock = new Mock<IShippersRepository>();
        _mapperMock = new Mock<IMapper>();
        _service = new ShippersService(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllShippersAsync_ReturnsMappedDtos()
    {
        // Arrange
        var shippers = new List<MongoShipper>
        {
            new(),
            new()
        };

        var expectedDtos = new List<GetShipperDto>
        {
            new(),
            new()
        };

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(shippers);

        _mapperMock.Setup(m => m.Map<IEnumerable<GetShipperDto>>(shippers))
            .Returns(expectedDtos);

        // Act
        var result = await _service.GetAllShippersAsync(CancellationToken.None);

        // Assert
        Assert.Equal(expectedDtos, result);
        _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<GetShipperDto>>(shippers), Times.Once);
    }
}