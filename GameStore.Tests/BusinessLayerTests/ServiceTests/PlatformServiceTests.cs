using AutoMapper;
using GameStore.BLL.DTOs.Games;
using GameStore.BLL.DTOs.Platforms;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Interfaces.Repositories;
using GameStore.BLL.Services.EntityServices;
using GameStore.Domain.Entities.CoreEntities;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests;

public class PlatformServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IGuidProvider> _guidProvider = new();
    private readonly PlatformService _platformService;

    public PlatformServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _guidProvider.Setup(g => g.NewGuid()).Returns(Guid.NewGuid());
        _platformService = new PlatformService(_guidProvider.Object, _unitOfWorkMock.Object, _mapperMock.Object);
        Mock<IPlatformRepository> platformRepositoryMock = new();
        _unitOfWorkMock.Setup(uow => uow.Platforms).Returns(platformRepositoryMock.Object);
    }

    [Fact]
    public async Task CreatePlatformAsync_ShouldCreatePlatform()
    {
        // Arrange
        var platformCreateDto = new PlatformCreateDto { Type = "test1" };
        var platform = new Platform { Id = Guid.NewGuid(), Type = "test1" };
        var platformDto = new PlatformDto { Id = platform.Id, Type = "test1" };

        _mapperMock.Setup(m => m.Map<Platform>(platformCreateDto)).Returns(platform);
        _mapperMock.Setup(m => m.Map<PlatformDto>(platform)).Returns(platformDto);

        // Act
        var result = await _platformService.CreatePlatformAsync(platformCreateDto, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.Platforms.AddPlatformAsync(platform, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(CancellationToken.None), Times.Once);
        Assert.Equal(platformDto, result);
    }

    [Fact]
    public async Task DeletePlatformAsync_ShouldDeletePlatform()
    {
        // Arrange
        var platformId = Guid.NewGuid();
        _unitOfWorkMock.Setup(x => x.Platforms.PlatformExistsAsync(platformId, CancellationToken.None)).ReturnsAsync(true);

        // Act
        await _platformService.DeletePlatformAsync(platformId, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.Platforms.DeletePlatformAsync(platformId, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetAllPlatformsAsync_ShouldReturnAllPlatforms()
    {
        // Arrange
        var platforms = new List<Platform>
        {
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() },
        };

        var platformDtos = platforms.Select(p => new PlatformDto { Id = p.Id, Type = p.Type }).ToList();

        _unitOfWorkMock.Setup(uow => uow.Platforms.GetAllPlatformsAsync(CancellationToken.None)).ReturnsAsync(platforms);
        _mapperMock.Setup(m => m.Map<IEnumerable<PlatformDto>>(platforms)).Returns(platformDtos);

        // Act
        var result = await _platformService.GetAllPlatformsAsync(CancellationToken.None);

        // Assert
        Assert.Equal(platformDtos.ToList()[0].Id, result.ToList()[0].Id);
    }

    [Fact]
    public async Task GetPlatformByIdAsync_ShouldReturnPlatform()
    {
        // Arrange
        var platformId = Guid.NewGuid();
        var platform = new Platform { Id = platformId };
        var platformDto = new PlatformDto { Id = platformId };

        _unitOfWorkMock.Setup(uow => uow.Platforms.GetPlatformByIdAsync(platformId, CancellationToken.None)).ReturnsAsync(platform);
        _mapperMock.Setup(m => m.Map<PlatformDto>(platform)).Returns(platformDto);

        // Act
        var result = await _platformService.GetPlatformByIdAsync(platformId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(platformDto, result);
    }

    [Fact]
    public async Task UpdatePlatformAsync_ShouldUpdatePlatform()
    {
        // Arrange
        var platformDto = new PlatformDto { Id = Guid.NewGuid(), Type = "xD" };
        var platform = new Platform { Id = Guid.NewGuid(), Type = "xD" };

        _mapperMock.Setup(m => m.Map<Platform>(platformDto)).Returns(platform);
        _unitOfWorkMock.Setup(x => x.Platforms.PlatformExistsAsync(platform.Id, CancellationToken.None)).ReturnsAsync(true);

        // Act
        await _platformService.UpdatePlatformAsync(platformDto, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.Platforms.UpdatePlatformAsync(platform), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetGamesByPlatformIdAsync_ShouldReturnGames()
    {
        // Arrange
        var platformId = Guid.NewGuid();
        var gameList = new List<Game> { new() { Id = Guid.NewGuid(), Key = "key", Name = "game" } };
        var platform = new Platform
        {
            Id = platformId,
            Games = gameList,
        };
        var gameDto = new GameDto { Id = gameList[0].Id, Name = "game", Key = "key" };

        _unitOfWorkMock.Setup(uow => uow.Platforms.GetPlatformByIdAsync(platformId, CancellationToken.None)).ReturnsAsync(platform);
        _mapperMock.Setup(m => m.Map<GameDto>(It.IsAny<Game>())).Returns(gameDto);

        // Act
        var result = await _platformService.GetGamesByPlatformIdAsync(platformId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(gameDto, result.First());
    }

    [Fact]
    public async Task GetGamesByPlatformIdAsync_ThrowsArgumentException_WhenPlatformNotFound()
    {
        // Arrange
        var guid = Guid.NewGuid();
        _unitOfWorkMock.Setup(uow => uow.Platforms.GetPlatformByIdAsync(guid, CancellationToken.None)).ReturnsAsync((Platform)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        await _platformService.GetGamesByPlatformIdAsync(guid, CancellationToken.None));
        Assert.Contains($"{guid}", exception.Message);
    }

    [Fact]
    public async Task UpdatePlatformAsync_ShouldThrowKeyNotFoundException_WhenPlatformNotExist()
    {
        // Arrange
        var platformDto = new PlatformDto { Id = Guid.NewGuid(), Type = "xD" };
        var platform = new Platform { Id = Guid.NewGuid(), Type = "xD" };

        _mapperMock.Setup(m => m.Map<Platform>(platformDto)).Returns(platform);
        _unitOfWorkMock.Setup(x => x.Platforms.PlatformExistsAsync(platform.Id, CancellationToken.None)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _platformService.UpdatePlatformAsync(platformDto, CancellationToken.None));
    }

    [Fact]
    public async Task GetPlatformsByIdsAsync_ReturnsGenres_WhenIdsAreValid()
    {
        // Arrange
        var platformIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var platforms = new List<Platform>
        {
            new() { Id = platformIds[0], Type = "Genre1" },
            new() { Id = platformIds[1], Type = "Genre2" },
        };

        _unitOfWorkMock.Setup(uow => uow.Platforms.GetPlatformByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(
                new Func<Guid, CancellationToken, Platform>((id, _) => platforms.Find(platform => platform.Id == id)));

        // Act
        var result = await _platformService.GetPlatformsByIdsAsync(platformIds, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.NotNull(result.FirstOrDefault(g => g.Id == platformIds[0]));
        Assert.NotNull(result.FirstOrDefault(g => g.Id == platformIds[1]));
    }

    [Fact]
    public async Task GetPlatformsByIdsAsync_ThrowsKeyNotFoundException_WhenAGenreIdIsNotFound()
    {
        // Arrange
        var platformIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        _unitOfWorkMock.Setup(uow => uow.Platforms.GetPlatformByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync((Platform)null!);

        // Act
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _platformService.GetPlatformsByIdsAsync(platformIds, CancellationToken.None));
    }
}
