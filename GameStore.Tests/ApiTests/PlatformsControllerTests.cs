using GameStore.BLL.DTOs.Games;
using GameStore.BLL.DTOs.Platforms;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Tests.ApiTests;

public class PlatformsControllerTests
{
    private readonly Mock<IPlatformService> _platformServiceMock;
    private readonly PlatformsController _controller;

    public PlatformsControllerTests()
    {
        _platformServiceMock = new Mock<IPlatformService>();
        _controller = new PlatformsController(_platformServiceMock.Object);
    }

    [Fact]
    public async Task CreatePlatformAsync_ReturnsCreatedAtActionResult_WithPlatformDto()
    {
        // Arrange
        var platformCreateDto = new PlatformCreateDto { Type = "Test Platform" };
        var platformRequest = new PlatformCreateRequest()
        {
            Platform = platformCreateDto,
        };
        var platformDto = new PlatformDto { Id = Guid.NewGuid(), Type = "Test Platform" };
        _platformServiceMock.Setup(s => s.CreatePlatformAsync(platformCreateDto, CancellationToken.None))
                            .ReturnsAsync(platformDto);

        // Act
        var result = await _controller.CreatePlatform(platformRequest, CancellationToken.None);

        // Assert
        var createdAtActionResult = result.Result as CreatedAtActionResult;
        Assert.NotNull(createdAtActionResult);
        Assert.Equal("GetPlatformById", createdAtActionResult.ActionName);
        Assert.Equal(platformDto.Id, createdAtActionResult.RouteValues["id"]);

        var returnedPlatform = createdAtActionResult.Value as PlatformDto;
        Assert.NotNull(returnedPlatform);
        Assert.Equal(platformDto.Type, returnedPlatform.Type);
    }

    [Fact]
    public async Task DeletePlatformAsync_ReturnsNoContentResult()
    {
        // Arrange
        var platformId = Guid.NewGuid();
        _platformServiceMock.Setup(s => s.DeletePlatformAsync(platformId, CancellationToken.None)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeletePlatform(platformId, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task GetAllPlatformsAsync_ReturnsOkResult_WithPlatforms()
    {
        // Arrange
        var platforms = new List<PlatformDto>
            {
                new() { Id = Guid.NewGuid(), Type = "Platform 1" },
                new() { Id = Guid.NewGuid(), Type = "Platform 2" },
            };
        _platformServiceMock.Setup(s => s.GetAllPlatformsAsync(CancellationToken.None)).ReturnsAsync(platforms);

        // Act
        var result = await _controller.GetAllPlatforms(CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        var returnedPlatforms = okResult.Value as IEnumerable<PlatformDto>;
        Assert.NotNull(returnedPlatforms);
        Assert.Equal(2, returnedPlatforms.Count());
    }

    [Fact]
    public async Task GetPlatformByIdAsync_ReturnsOkResult_WithPlatformDto()
    {
        // Arrange
        var platformId = Guid.NewGuid();
        var platformDto = new PlatformDto { Id = platformId, Type = "Test Platform" };
        _platformServiceMock.Setup(s => s.GetPlatformByIdAsync(platformId, CancellationToken.None)).ReturnsAsync(platformDto);

        // Act
        var result = await _controller.GetPlatformById(platformId, CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        var returnedPlatform = okResult.Value as PlatformDto;
        Assert.NotNull(returnedPlatform);
        Assert.Equal(platformDto.Id, returnedPlatform.Id);
    }

    [Fact]
    public async Task GetPlatformByIdAsync_ReturnsNotFoundResult()
    {
        // Arrange
        var platformId = Guid.NewGuid();
        _platformServiceMock.Setup(s => s.GetPlatformByIdAsync(platformId, CancellationToken.None)).ReturnsAsync((PlatformDto)null!);

        // Act
        var result = await _controller.GetPlatformById(platformId, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdatePlatformAsync_ReturnsNoContentResult()
    {
        // Arrange
        var platformDto = new PlatformDto { Id = Guid.NewGuid(), Type = "Updated Platform" };
        var platformRequest = new PlatformRequest()
        {
            Platform = platformDto,
        };
        _platformServiceMock.Setup(s => s.UpdatePlatformAsync(platformDto, CancellationToken.None)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdatePlatform(platformRequest, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task GetGamesByPlatformIdAsync_ReturnsOkResult_WithGames()
    {
        // Arrange
        var platformId = Guid.NewGuid();
        var games = new List<GameDto>
            {
                new() { Id = Guid.NewGuid(), Name = "Game 1" },
                new() { Id = Guid.NewGuid(), Name = "Game 2" },
            };
        _platformServiceMock.Setup(s => s.GetGamesByPlatformIdAsync(platformId, CancellationToken.None)).ReturnsAsync(games);

        // Act
        var result = await _controller.GetGamesByPlatformId(platformId, CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        var returnedGames = okResult.Value as IEnumerable<GameDto>;
        Assert.NotNull(returnedGames);
        Assert.Equal(2, returnedGames.Count());
    }
}