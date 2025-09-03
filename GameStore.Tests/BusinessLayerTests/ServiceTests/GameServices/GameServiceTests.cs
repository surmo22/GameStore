using AutoMapper;
using GameStore.BLL.DTOs.Games;
using GameStore.BLL.DTOs.Genres;
using GameStore.BLL.DTOs.Platforms;
using GameStore.BLL.DTOs.Publisher;
using GameStore.BLL.GamesQueryPipeline.Interfaces;
using GameStore.BLL.Interfaces.GameServices;
using GameStore.BLL.Interfaces.GameServices.ImageServices;
using GameStore.BLL.Interfaces.Repositories;
using GameStore.BLL.Interfaces.Security;
using GameStore.BLL.Services.EntityServices;
using GameStore.Domain.Entities.CoreEntities;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.GameServices;

public class GameServiceTests
{
    private readonly GameService _gameService;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IGameSetupService> _gameSetupService;
    private readonly Mock<IGamePipelineFactory> _gamePipelineFactory;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly Mock<IGameImageService> _gameImageServiceMock = new();

    public GameServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        var gameCountService = new Mock<IGameCountService>();
        _gameSetupService = new Mock<IGameSetupService>();
        _gamePipelineFactory = new Mock<IGamePipelineFactory>();
        _mapperMock = new Mock<IMapper>();
        _gameService = new GameService(
            _unitOfWorkMock.Object,
            _gameSetupService.Object,
            _mapperMock.Object,
            gameCountService.Object,
            _gamePipelineFactory.Object,
            _currentUserServiceMock.Object,
            _gameImageServiceMock.Object);
    }

    [Fact]
    public async Task CreateGameAsync_ShouldReturnGameDto_WhenSuccessful()
    {
        // Arrange
        var genres = new List<Guid>
        {
            Guid.NewGuid(),
            Guid.NewGuid(),
        };
        var platforms = new List<Guid> { Guid.NewGuid() };
        var gameCreateRequest = new GameCreateRequestDto
        {
            Game = new GameCreateDto { Name = "Test Game" },
            Genres = genres,
            Platforms = platforms,
        };

        var gameEntity = new Game { Name = "Test Game" };
        var gameDto = new GameDto { Id = gameEntity.Id, Name = gameEntity.Name };

        _mapperMock.Setup(m => m.Map<Game>(gameCreateRequest.Game)).Returns(gameEntity);
        _mapperMock.Setup(m => m.Map<GameDto>(gameEntity)).Returns(gameDto);
        _gameSetupService.Setup(gss => gss.BuildGameAsync(gameEntity, gameCreateRequest, CancellationToken.None))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.Games.AddGameAsync(gameEntity, CancellationToken.None))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.CompleteAsync(CancellationToken.None)).Returns(Task.CompletedTask);

        // Mocking genre assignment
        _unitOfWorkMock.Setup(uow => uow.Genres.GetGenreByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(new Genre());
        _unitOfWorkMock.Setup(uow => uow.Platforms.GetPlatformByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(new Platform());

        // Act
        var result = await _gameService.CreateGameAsync(gameCreateRequest, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(gameDto.Name, result.Name);
        Assert.Equal(gameDto.Id, result.Id);
        Assert.Equal(gameDto.Description, result.Description);
        _unitOfWorkMock.Verify(uow => uow.Games.AddGameAsync(gameEntity, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdateGameAsync_ShouldReturnGameDto_WhenSuccessful()
    {
        // Arrange
        var ids = new List<Guid>();
        var gameUpdateRequest = new GameUpdateRequestDto
        {
            Game = new GameDto { Id = Guid.NewGuid(), Name = "Updated Game" },
            Genres = ids,
            Platforms = ids,
        };

        var gameEntity = new Game { Id = gameUpdateRequest.Game.Id, Name = "Old Game", IsDeleted = true };
        var gameDto = new GameDto { Id = gameEntity.Id, Name = gameEntity.Name };

        _mapperMock.Setup(m => m.Map<Game>(It.IsAny<GameDto>())).Returns(gameEntity);
        _mapperMock.Setup(m => m.Map<GameDto>(It.IsAny<Game>())).Returns(gameDto);
        _currentUserServiceMock.Setup(c => c.HasPermissionToAsync(It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _gameSetupService.Setup(gss => gss.BuildGameAsync(gameEntity, gameUpdateRequest, CancellationToken.None))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.Games.UpdateGameAsync(gameEntity, CancellationToken.None))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.CompleteAsync(CancellationToken.None)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.Genres.GetGenreByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(new Genre());
        _unitOfWorkMock.Setup(uow => uow.Platforms.GetPlatformByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(new Platform());
        _unitOfWorkMock.Setup(uow => uow.Games.GetGameByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(gameEntity);
        _unitOfWorkMock.Setup(uow => uow.Games.GameExistsAsync(gameEntity.Id, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var result = await _gameService.UpdateGameAsync(gameUpdateRequest, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(gameDto.Name, result.Name);
        _unitOfWorkMock.Verify(uow => uow.Games.UpdateGameAsync(It.IsAny<Game>(), CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(CancellationToken.None), Times.Once);
    }
    
    [Fact]
    public async Task UpdateGameAsync_ThrowsException_WhenNoPermission()
    {
        // Arrange
        var ids = new List<Guid>();
        var gameUpdateRequest = new GameUpdateRequestDto
        {
            Game = new GameDto { Id = Guid.NewGuid(), Name = "Updated Game" },
            Genres = ids,
            Platforms = ids,
        };

        var gameEntity = new Game { Id = gameUpdateRequest.Game.Id, Name = "Old Game", IsDeleted = true };
        var gameDto = new GameDto { Id = gameEntity.Id, Name = gameEntity.Name };

        _mapperMock.Setup(m => m.Map<Game>(It.IsAny<GameDto>())).Returns(gameEntity);
        _mapperMock.Setup(m => m.Map<GameDto>(It.IsAny<Game>())).Returns(gameDto);
        _currentUserServiceMock.Setup(c => c.HasPermissionToAsync(It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _gameSetupService.Setup(gss => gss.BuildGameAsync(gameEntity, gameUpdateRequest, CancellationToken.None))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.Games.UpdateGameAsync(gameEntity, CancellationToken.None))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.CompleteAsync(CancellationToken.None)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.Genres.GetGenreByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(new Genre());
        _unitOfWorkMock.Setup(uow => uow.Platforms.GetPlatformByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(new Platform());
        _unitOfWorkMock.Setup(uow => uow.Games.GetGameByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(gameEntity);
        _unitOfWorkMock.Setup(uow => uow.Games.GameExistsAsync(gameEntity.Id, CancellationToken.None))
            .ReturnsAsync(true);

        // Act && Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _gameService.UpdateGameAsync(gameUpdateRequest, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteGameAsync_ShouldCallDeleteGame()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        _unitOfWorkMock.Setup(uow => uow.Games.DeleteGameAsync(gameId, CancellationToken.None))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.Games.GetGameByKeyAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Game()
            {
                Id = gameId,
            });
        _unitOfWorkMock.Setup(uow => uow.CompleteAsync(CancellationToken.None)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.Games.GameExistsAsync(gameId, CancellationToken.None)).ReturnsAsync(true);
        _mapperMock.Setup(m => m.Map<GameDto>(It.IsAny<Game>())).Returns(new GameDto()
        {
            Id = gameId,
        });

        // Act
        await _gameService.DeleteGameAsync(gameId.ToString(), CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.Games.DeleteGameAsync(gameId, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetGameByIdAsync_ShouldReturnGameDto_WhenFound()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var gameEntity = new Game { Id = gameId, Name = "Found Game" };
        var gameDto = new GameDto { Id = gameId, Name = gameEntity.Name };

        _unitOfWorkMock.Setup(uow => uow.Games.GetGameByIdAsync(gameId, CancellationToken.None))
            .ReturnsAsync(gameEntity);
        _mapperMock.Setup(m => m.Map<GameDto>(gameEntity)).Returns(gameDto);

        // Act
        var result = await _gameService.GetGameByIdAsync(gameId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(gameDto.Name, result.Name);
    }
    
    [Fact]
    public async Task GetGameByIdAsync_ShouldThrows_WhenNoPermission()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var gameEntity = new Game { Id = gameId, Name = "Found Game", IsDeleted = true };
        var gameDto = new GameDto { Id = gameId, Name = gameEntity.Name };

        _unitOfWorkMock.Setup(uow => uow.Games.GetGameByIdAsync(gameId, CancellationToken.None))
            .ReturnsAsync(gameEntity);
        _mapperMock.Setup(m => m.Map<GameDto>(gameEntity)).Returns(gameDto);
        _currentUserServiceMock.Setup(c => c.HasPermissionToAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        // Act && Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _gameService.GetGameByIdAsync(gameId, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateGameStockAsync_ShouldUpdateStock_WhenSuccessful()
    {
        // Arrange
        _unitOfWorkMock.Setup(u =>
                u.Games.UpdateUnitsInStockAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _unitOfWorkMock.Setup(u => u.CompleteAsync(It.IsAny<CancellationToken>()));
        
        // Act
        await _gameService.UpdateGameStockAsync(Guid.NewGuid(), 10, CancellationToken.None);
        
        // Assert
        _unitOfWorkMock.Verify(
            u => u.Games.UpdateUnitsInStockAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(u => u.CompleteAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllGamesWithoutFilters_ReturnsAllGames()
    {
        // Arrange
        var gamesEntity = new List<Game>
        {
            new() { Id = Guid.NewGuid(), Name = "Game 1" },
            new() { Id = Guid.NewGuid(), Name = "Game 2" },
        };
        
        var gamesDto = new List<GameDto>
        {
            new() { Id = gamesEntity[0].Id, Name = gamesEntity[0].Name },
            new() { Id = gamesEntity[1].Id, Name = gamesEntity[1].Name },
        };
        
        _gamePipelineFactory.Setup(g => g.Create(It.IsAny<GameFilterRequest>(), It.IsAny<bool>()))
            .Returns(new Mock<IGamePipeline>().Object);
        
        _unitOfWorkMock.Setup(uow => uow.Games.GetAllGamesAsync(It.IsAny<IGamePipeline>(), CancellationToken.None)).ReturnsAsync(gamesEntity);
        _mapperMock.Setup(m => m.Map<IEnumerable<GameDto>>(gamesEntity)).Returns(gamesDto);
        
        // Act
        var result = await _gameService.GetAllGamesWithoutFilters(CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(gamesDto, result);
    }

    [Fact]
    public async Task GetAllGamesAsync_ShouldReturnListOfGameDtos()
    {
        // Arrange
        var gamesEntity = new List<Game>
        {
            new() { Id = Guid.NewGuid(), Name = "Game 1" },
            new() { Id = Guid.NewGuid(), Name = "Game 2" },
        };

        var gamesDto = new List<GameDto>
        {
            new() { Id = gamesEntity[0].Id, Name = gamesEntity[0].Name },
            new() { Id = gamesEntity[1].Id, Name = gamesEntity[1].Name },
        };
        
        _gamePipelineFactory.Setup(g => g.Create(It.IsAny<GameFilterRequest>(), It.IsAny<bool>()))
            .Returns(new Mock<IGamePipeline>().Object);
        _unitOfWorkMock.Setup(uow => uow.Games.GetAllGamesAsync(It.IsAny<IGamePipeline>(), CancellationToken.None)).ReturnsAsync(gamesEntity);
        _mapperMock.Setup(m => m.Map<IEnumerable<GameDto>>(gamesEntity)).Returns(gamesDto);

        // Act
        var result = await _gameService.GetAllGamesAsync(new GameFilterRequest()
        {
            PageCount = "10",
        }, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(gamesDto.Count, result.Games.Count());
    }

    [Fact]
    public async Task GetGenresByGameKeyAsync_ShouldReturnListOfGenreDtos()
    {
        // Arrange
        var gameKey = "test-key";
        var genreId = Guid.NewGuid();
        var genres = new List<Genre> { new() { Id = genreId, Name = "Genre 1" } };
        var gameEntity = new Game { Key = gameKey, Genres = genres };
        var genreDto = new GenreDto { Id = genreId, Name = "Genre 1" };

        _unitOfWorkMock.Setup(uow => uow.Games.GetGameByKeyAsync(gameKey, CancellationToken.None))
            .ReturnsAsync(gameEntity);
        _unitOfWorkMock.Setup(uow => uow.Genres.GetGenreByIdAsync(genreId, CancellationToken.None))
            .ReturnsAsync(gameEntity.Genres.First());
        _mapperMock.Setup(m => m.Map<GenreDto>(It.IsAny<Genre>())).Returns(genreDto);

        // Act
        var result = await _gameService.GetGenresByGameKeyAsync(gameKey, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(genreDto.Name, result.First().Name);
    }

    [Fact]
    public async Task GetGenresByGameKeyAsync_ShouldThrowArgumentException_WhenGameNotFound()
    {
        // Arrange
        var gameKey = "invalid-key";
        _unitOfWorkMock.Setup(uow => uow.Games.GetGameByKeyAsync(gameKey, CancellationToken.None))
            .ReturnsAsync((Game)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _gameService.GetGenresByGameKeyAsync(gameKey, CancellationToken.None));
        Assert.Contains($"{gameKey}", exception.Message);
    }

    [Fact]
    public async Task GetPlatformsByGameKeyAsync_ShouldReturnListOfPlatformDtos()
    {
        // Arrange
        var gameKey = "test-key";
        var platformId = Guid.NewGuid();
        var platforms = new List<Platform> { new() { Id = platformId, Type = "Platform 1" } };
        var gameEntity = new Game { Key = gameKey, Platforms = platforms };
        var platformDto = new PlatformDto { Id = platformId, Type = "Platform 1" };

        _unitOfWorkMock.Setup(uow => uow.Games.GetGameByKeyAsync(gameKey, CancellationToken.None))
            .ReturnsAsync(gameEntity);
        _unitOfWorkMock.Setup(uow => uow.Platforms.GetPlatformByIdAsync(platformId, CancellationToken.None))
            .ReturnsAsync(gameEntity.Platforms.First());
        _mapperMock.Setup(m => m.Map<PlatformDto>(It.IsAny<Platform>())).Returns(platformDto);

        // Act
        var result = await _gameService.GetPlatformsByGameKeyAsync(gameKey, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(platformDto.Type, result.First().Type);
    }

    [Fact]
    public async Task GetPlatformsByGameKeyAsync_ShouldThrowArgumentException_WhenGameNotFound()
    {
        // Arrange
        var gameKey = "invalid-key";
        _unitOfWorkMock.Setup(uow => uow.Games.GetGameByKeyAsync(gameKey, CancellationToken.None))
            .ReturnsAsync((Game)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _gameService.GetPlatformsByGameKeyAsync(gameKey, CancellationToken.None));
        Assert.Contains($"{gameKey}", exception.Message);
    }

    [Fact]
    public async Task GetGameByKeyAsync_ShouldReturnGameDto_WhenFound()
    {
        // Arrange
        var gameKey = "test-key";
        var gameEntity = new Game { Key = gameKey, Name = "Found Game" };
        var gameDto = new GameDto { Key = gameKey, Name = gameEntity.Name };

        _unitOfWorkMock.Setup(uow => uow.Games.GetGameByKeyAsync(gameKey, CancellationToken.None))
            .ReturnsAsync(gameEntity);
        _mapperMock.Setup(m => m.Map<GameDto>(gameEntity)).Returns(gameDto);

        // Act
        var result = await _gameService.GetGameByKeyAsync(gameKey, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(gameDto.Name, result.Name);
    }
    
    [Fact]
    public async Task GetGameByKeyAsync_Throws_WhenNoPermission()
    {
        // Arrange
        var gameKey = "test-key";
        var gameEntity = new Game { Key = gameKey, Name = "Found Game", IsDeleted = true};
        var gameDto = new GameDto { Key = gameKey, Name = gameEntity.Name };

        _unitOfWorkMock.Setup(uow => uow.Games.GetGameByKeyAsync(gameKey, CancellationToken.None))
            .ReturnsAsync(gameEntity);
        _currentUserServiceMock.Setup(c => c.HasPermissionToAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mapperMock.Setup(m => m.Map<GameDto>(gameEntity)).Returns(gameDto);

        // Act && Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _gameService.GetGameByKeyAsync(gameKey, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateGame_ThrowsKeyNotFoundException_WhenGameNotExist()
    {
        // Arrange
        var ids = new List<Guid>();
        var gameUpdateRequest = new GameUpdateRequestDto
        {
            Game = new GameDto { Id = Guid.NewGuid(), Name = "Updated Game" },
            Genres = ids,
            Platforms = ids,
        };

        var gameEntity = new Game { Id = gameUpdateRequest.Game.Id, Name = "Old Game" };
        var gameDto = new GameDto { Id = gameEntity.Id, Name = gameEntity.Name };

        _mapperMock.Setup(m => m.Map<Game>(gameUpdateRequest.Game)).Returns(gameEntity);
        _mapperMock.Setup(m => m.Map<GameDto>(gameEntity)).Returns(gameDto);
        _unitOfWorkMock.Setup(uow => uow.Games.UpdateGameAsync(gameEntity, CancellationToken.None))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.CompleteAsync(CancellationToken.None)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.Genres.GetGenreByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(new Genre());
        _unitOfWorkMock.Setup(uow => uow.Platforms.GetPlatformByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(new Platform());
        _unitOfWorkMock.Setup(uow => uow.Games.GameExistsAsync(gameEntity.Id, CancellationToken.None))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _gameService.UpdateGameAsync(gameUpdateRequest, CancellationToken.None));
    }

    [Fact]
    public async Task GetPublisherByGameKeyAsync_ShouldReturnPublisherDto_WhenFound()
    {
        // Arrange
        var gameKey = "test-key";
        var publisherId = Guid.NewGuid();
        var publisher = new Publisher { Id = publisherId, CompanyName = "Publisher 1" };
        var gameEntity = new Game { Key = gameKey, Publisher = publisher };
        var publisherDto = new PublisherDto { Id = publisherId, CompanyName = "Publisher 1" };

        _unitOfWorkMock.Setup(uow => uow.Games.GetGameByKeyAsync(gameKey, CancellationToken.None))
            .ReturnsAsync(gameEntity);
        _mapperMock.Setup(m => m.Map<PublisherDto>(It.IsAny<Publisher>())).Returns(publisherDto);

        // Act
        var result = await _gameService.GetPublisherByGameKeyAsync(gameKey, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(publisherDto.CompanyName, result.CompanyName);
    }

    [Fact]
    public async Task GetPublisherByGameKeyAsync_ShouldThrowArgumentException_WhenGameNotFound()
    {
        // Arrange
        var gameKey = "invalid-key";
        _unitOfWorkMock.Setup(uow => uow.Games.GetGameByKeyAsync(gameKey, CancellationToken.None))
            .ReturnsAsync((Game)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _gameService.GetPublisherByGameKeyAsync(gameKey, CancellationToken.None));

        Assert.Contains($"{gameKey}", exception.Message);
    }
    
    [Fact]
    public async Task GetImageByGameKeyAsync_ReturnsImage_WhenGameExists()
    {
        // Arrange
        var key = "some-key";
        var imageUrl = "game/image.jpg";
        var imageBytes = new byte[] { 1, 2, 3 };
        var contentType = "image/jpeg";
        var game = new Game { ImageUrl = imageUrl };
        
        _unitOfWorkMock.Setup(x => x.Games.GetGameByKeyAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(game);
        
        _gameImageServiceMock
            .Setup(x => x.GetImage(imageUrl, It.IsAny<CancellationToken>()))
            .ReturnsAsync((imageBytes, contentType));

        // Act
        var (content, returnedContentType) = await _gameService.GetImageByGameKeyAsync(key, CancellationToken.None);

        // Assert
        Assert.Equal(imageBytes, content);
        Assert.Equal(contentType, returnedContentType);
    }

    [Fact]
    public async Task GetImageByGameKeyAsync_Throws_WhenGameNotFound()
    {
        // Arrange
        var key = "nonexistent-key";
        
        _unitOfWorkMock.Setup(x => x.Games.GetGameByKeyAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Game)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _gameService.GetImageByGameKeyAsync(key, CancellationToken.None));

        Assert.Contains(key, ex.Message);
    }
}
