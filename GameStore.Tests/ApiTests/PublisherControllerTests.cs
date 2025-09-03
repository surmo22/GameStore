using GameStore.BLL.DTOs.Games;
using GameStore.BLL.DTOs.Publisher;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Tests.ApiTests;

public class PublisherControllerTests
{
    private readonly Mock<IPublisherService> _publisherService;
    private readonly PublisherController _controller;

    public PublisherControllerTests()
    {
        _publisherService = new Mock<IPublisherService>();
        _controller = new PublisherController(_publisherService.Object);
    }

    [Fact]
    public async Task CreatePublisher_ReturnsOkWithPublisherDto()
    {
        // Arrange
        var publisherCreateDto = new PublisherCreateDto() { CompanyName = "test-publisher" };
        var publisherCreateRequest = new PublisherCreateRequest { Publisher = publisherCreateDto };
        var publisherDto = new PublisherDto { CompanyName = publisherCreateDto.CompanyName };
        _publisherService.Setup(s => s.CreatePublisherAsync(publisherCreateRequest.Publisher, CancellationToken.None)).ReturnsAsync(publisherDto);

        // Act
        var result = await _controller.CreatePublisher(publisherCreateRequest, CancellationToken.None);

        // Assert
        var createdAtActionResult = result.Result as CreatedAtActionResult;
        _publisherService.Verify(p => p.CreatePublisherAsync(It.IsAny<PublisherCreateDto>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(createdAtActionResult);
        Assert.Equal("GetPublisherByCompanyName", createdAtActionResult.ActionName);
    }

    [Fact]
    public async Task DeletePublisher_ReturnsNoContent_WhenSuccessful()
    {
        // Arrange
        var publisherId = Guid.NewGuid();
        _publisherService.Setup(s => s.DeletePublisherAsync(publisherId, CancellationToken.None)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeletePublisher(publisherId, CancellationToken.None);

        // Assert
        var noContentResult = result as NoContentResult;
        Assert.NotNull(noContentResult);
        _publisherService.Verify(s => s.DeletePublisherAsync(publisherId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetAllPublishers_ReturnsOkWithListOfPublisherDto()
    {
        // Arrange
        var publishers = new List<PublisherDto> { new() { CompanyName = "test-publisher" } };
        _publisherService.Setup(s => s.GetAllPublishersAsync(CancellationToken.None)).ReturnsAsync(publishers);

        // Act
        var result = await _controller.GetAllPublishers(CancellationToken.None);

        // Assert
        var okObjectResult = result.Result as OkObjectResult;
        _publisherService.Verify(p => p.GetAllPublishersAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(publishers, okObjectResult.Value);
    }

    [Fact]
    public async Task GetPublisherByCompanyName_ReturnsOkWithPublisherDto()
    {
        // Arrange
        var companyName = "test-publisher";
        var publisherDto = new PublisherDto { CompanyName = companyName };
        _publisherService.Setup(s => s.GetPublisherByCompanyNameAsync(companyName, CancellationToken.None)).ReturnsAsync(publisherDto);

        // Act
        var result = await _controller.GetPublisherByCompanyName(companyName, CancellationToken.None);

        // Assert
        var okObjectResult = result.Result as OkObjectResult;
        _publisherService.Verify(p => p.GetPublisherByCompanyNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(okObjectResult);
        Assert.Equal(publisherDto, okObjectResult.Value);
    }

    [Fact]
    public async Task UpdatePublisher_ReturnsNoContent_WhenSuccessful()
    {
        // Arrange
        var publisherDto = new PublisherDto { CompanyName = "test-publisher" };
        var publisherRequest = new PublisherRequest { Publisher = publisherDto };
        _publisherService.Setup(s => s.UpdatePublisherAsync(publisherDto, CancellationToken.None)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdatePublisher(publisherRequest, CancellationToken.None);

        // Assert
        var noContentResult = result as NoContentResult;
        Assert.NotNull(noContentResult);
        _publisherService.Verify(s => s.UpdatePublisherAsync(publisherDto, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetGamesByPublisher_ReturnsOkWithListOfGameDto()
    {
        // Arrange
        var companyName = "test-publisher";
        var games = new List<GameDto> { new() { Name = "test-game" } };
        _publisherService.Setup(s => s.GetGamesByPublisherCompanyNameAsync(companyName, CancellationToken.None)).ReturnsAsync(games);

        // Act
        var result = await _controller.GetGamesByPublisherName(companyName, CancellationToken.None);

        // Assert
        var okObjectResult = result.Result as OkObjectResult;
        _publisherService.Verify(p => p.GetGamesByPublisherCompanyNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(okObjectResult);
        Assert.Equal(games, okObjectResult.Value);
    }
}