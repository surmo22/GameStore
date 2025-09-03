using System.Security.Claims;
using GameStore.BLL.DTOs.Comment;
using GameStore.BLL.DTOs.Games;
using GameStore.BLL.DTOs.Genres;
using GameStore.BLL.DTOs.Platforms;
using GameStore.BLL.DTOs.Publisher;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.BLL.Interfaces.GameServices;
using GameStore.BLL.Interfaces.OrderServices;
using GameStore.Common.Constants;
using GameStore.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace GameStore.Tests.ApiTests;

public class GamesControllerTests : IDisposable
{
    private readonly Mock<IGameService> _gameServiceMock;
    private readonly GamesController _controller;
    private readonly Mock<IGameFileService> _gameFileServiceMock;
    private readonly Mock<IOrderService> _orderServiceMock;
    private readonly Mock<ICommentService> _commentServiceMock;
    private readonly Mock<IUserBanService> _userBanService;
    private readonly MemoryCache _memoryCache;

    public GamesControllerTests()
    {
        _gameServiceMock = new Mock<IGameService>();
        _gameFileServiceMock = new Mock<IGameFileService>();
        _orderServiceMock = new Mock<IOrderService>();
        _commentServiceMock = new Mock<ICommentService>();
        _userBanService = new Mock<IUserBanService>();
        _memoryCache = new MemoryCache(new MemoryCacheOptions());

        _controller = new GamesController(_gameServiceMock.Object,
            _gameFileServiceMock.Object,
            _orderServiceMock.Object,
            _commentServiceMock.Object,
            _memoryCache);
        var userId = Guid.NewGuid();
        var user = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        ], "mock"));
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext() { User = user}
        };
    }
    
    [Fact]
    public async Task GetGameImage_ReturnsFile_FromCache()
    {
        var gameKey = "key123";
        var expectedBytes = new byte[] { 1, 2, 3 };
        var expectedContentType = "image/png";
        var cacheKey = $"GameImage:{gameKey}";

        _memoryCache.Set(cacheKey, (expectedBytes, expectedContentType), TimeSpan.FromMinutes(10));

        var result = await _controller.GetGameImage(gameKey, CancellationToken.None);

        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal(expectedBytes, fileResult.FileContents);
        Assert.Equal(expectedContentType, fileResult.ContentType);
    }


    [Fact]
    public async Task GetGameImage_CallsServiceAndCaches_WhenNotInCache()
    {
        // Arrange
        var gameKey = "key123";
        var imageBytes = new byte[] { 4, 5, 6 };
        var contentType = "image/jpeg";
        var cacheKey = $"GameImage:{gameKey}";

        var serviceResult = (imageBytes, contentType);

        _gameServiceMock
            .Setup(s => s.GetImageByGameKeyAsync(gameKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.GetGameImage(gameKey, CancellationToken.None);

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal(imageBytes, fileResult.FileContents);
        Assert.Equal(contentType, fileResult.ContentType);

        _gameServiceMock.Verify(s => s.GetImageByGameKeyAsync(gameKey, It.IsAny<CancellationToken>()), Times.Once);

        // Verify that the cache now contains the item
        Assert.True(_memoryCache.TryGetValue(cacheKey, out (byte[] Content, string ContentType) cached));
        Assert.Equal(imageBytes, cached.Content);
        Assert.Equal(contentType, cached.ContentType);
    }

    [Fact]
    public async Task GetGameImage_ReturnsOk_WhenContentIsEmpty()
    {
        // Arrange
        var gameKey = "key123";
        var cacheKey = $"GameImage:{gameKey}";

        var emptyResult = (Content: Array.Empty<byte>(), ContentType: "image/png");

        _gameServiceMock
            .Setup(s => s.GetImageByGameKeyAsync(gameKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyResult);

        // Act
        var result = await _controller.GetGameImage(gameKey, CancellationToken.None);

        // Assert
        Assert.IsType<OkResult>(result);

        _gameServiceMock.Verify(s => s.GetImageByGameKeyAsync(gameKey, It.IsAny<CancellationToken>()), Times.Once);

        // Confirm cache is still empty for that key
        Assert.False(_memoryCache.TryGetValue(cacheKey, out _));
    }
    
    [Fact]
    public async Task GetAllGamesWithoutFilter_ReturnsOkWithGames()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var games = new List<GameDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Game 1" },
            new() { Id = Guid.NewGuid(), Name = "Game 2" }
        };
        _gameServiceMock.Setup(s => s.GetAllGamesWithoutFilters(cancellationToken))
            .ReturnsAsync(games);

        // Act
        var result = await _controller.GetAllGamesWithoutFilter(cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGames = Assert.IsAssignableFrom<IEnumerable<GameDto>>(okResult.Value);
        Assert.Equal(2, returnedGames.Count());
    }

    [Fact]
    public void GetPaginationOptions_ReturnsOkWithDescriptions()
    {
        // Act
        var result = _controller.GetPaginationOptions();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var options = Assert.IsAssignableFrom<List<string>>(okResult.Value);
        Assert.NotEmpty(options);
        // Optionally check if they match the enum descriptions
    }

    [Fact]
    public void GetSortingOptions_ReturnsOkWithDescriptions()
    {
        // Act
        var result = _controller.GetSortingOptions();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var options = Assert.IsAssignableFrom<List<SortingOptions>>(okResult.Value);
        Assert.NotEmpty(options);
    }

    [Fact]
    public void GetPublishDateOptions_ReturnsOkWithDescriptions()
    {
        // Act
        var result = _controller.GetPublishDateOptions();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var options = Assert.IsAssignableFrom<List<PublishingDateFilter>>(okResult.Value);
        Assert.NotEmpty(options);
    }

    [Fact]
    public async Task AddComment_AddsComment()
    {
        // arrange
        var request = new AddCommentRequestDto()
        {
            AddComment = new AddCommentDto()
            {
                Body = "test",
            },
            Action = "test",
        };

        _userBanService.Setup(u => u.IsUserBannedAsync(It.IsAny<Guid>(), CancellationToken.None)).ReturnsAsync(false);
        _commentServiceMock.Setup(s => s.AddCommentAsync(It.IsAny<Guid>(), request, "key", CancellationToken.None)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AddComment("key", request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, okResult.StatusCode);
    }

    [Fact]
    public async Task GetCommentsByGameKey_ReturnsComments()
    {
        // Arrange
        var comments = new List<GetCommentDto> { new() { Name = "test" }, new() { Name = "test2" } };
        _commentServiceMock.Setup(s => s.GetCommentsByGameKeyAsync("key", CancellationToken.None)).ReturnsAsync(comments);

        // Act
        var result = await _controller.GetCommentsByGameKey("key", CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.Equal(comments, okResult.Value);
    }

    [Fact]
    public async Task DeleteComment_DeletesComment()
    {
        // arrange
        string gameKey = "test";
        Guid id = Guid.NewGuid();
        _gameServiceMock.Setup(s => s.GetGameByKeyAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GameDto());
        _commentServiceMock.Setup(s => s.DeleteCommentAsync(id, CancellationToken.None)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteComment(gameKey, id, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<NoContentResult>(result);
        Assert.Equal(204, okResult.StatusCode);
    }

    [Fact]
    public async Task DeleteComment_ReturnsNotFound_WhenGameNotFound()
    {
        // arrange
        string gameKey = "test";
        Guid id = Guid.NewGuid();
        _commentServiceMock.Setup(s => s.DeleteCommentAsync(id, CancellationToken.None)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteComment(gameKey, id, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, okResult.StatusCode);
    }

    [Fact]
    public async Task CreateGameAsync_ReturnsCreatedResult_WithGameDto()
    {
        // Arrange
        var gameDto = new GameDto { Key = "test-game" };
        var gameCreateDto = new GameCreateDto { Key = gameDto.Key };
        var gameCreateRequestDto = new GameCreateRequestDto { Game = gameCreateDto };
        _gameServiceMock.Setup(s => s.CreateGameAsync(gameCreateRequestDto, CancellationToken.None)).ReturnsAsync(gameDto);

        // Act
        var result = await _controller.CreateGame(gameCreateRequestDto, CancellationToken.None);

        // Assert
        var createdAtActionResult = result.Result as CreatedAtActionResult;
        Assert.Equal("GetGameByKey", createdAtActionResult.ActionName);
        Assert.Equal(gameDto.Key, createdAtActionResult.RouteValues["gameKey"]);

        var returnedGameDto = createdAtActionResult.Value as GameDto;
        Assert.NotNull(returnedGameDto);
        Assert.Equal(gameDto.Key, returnedGameDto.Key);
    }

    [Fact]
    public async Task DeleteGameAsync_ReturnsNoContent_WhenSuccessful()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        _gameServiceMock.Setup(s => s.DeleteGameAsync(gameId.ToString(), CancellationToken.None)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteGame(gameId.ToString(), CancellationToken.None);

        // Assert
        var noContentResult = result as NoContentResult;
        Assert.NotNull(noContentResult);
        _gameServiceMock.Verify(s => s.DeleteGameAsync(gameId.ToString(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetAllGamesAsync_ReturnsOkResult_WithListOfGames()
    {
        // Arrange
        var games = new List<GameDto> { new() { Name = "Game 1" }, new() { Name = "Game 2" } };
        var paginatedGames = new PaginatedList<GameDto>()
        {
            Games = games,
        };

        _gameServiceMock.Setup(s => 
                s.GetAllGamesAsync(It.IsAny<GameFilterRequest>(), CancellationToken.None))
            .ReturnsAsync(paginatedGames);

        // Act
        var result = await _controller.GetAllGames(new GameFilterRequest(), CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.Equal(paginatedGames, okResult.Value);
    }

    [Fact]
    public async Task GetGameByKeyAsync_ReturnsNotFound_WhenGameDoesNotExist()
    {
        // Arrange
        var gameKey = "non-existent-key";
        _gameServiceMock.Setup(s => s.GetGameByKeyAsync(gameKey, CancellationToken.None)).ReturnsAsync((GameDto)null!);

        // Act
        var result = await _controller.GetGameByKey(gameKey, CancellationToken.None);

        // Assert
        var notFoundResult = result.Result as NotFoundObjectResult;
        Assert.Equal($"Game with key {gameKey} not found.", notFoundResult.Value);
    }

    [Fact]
    public async Task GetGameByKeyAsync_ReturnsGame()
    {
        // Arrange
        var gameKey = "key";
        var gameDto = new GameDto { Key = gameKey };
        _gameServiceMock.Setup(s => s.GetGameByKeyAsync(gameKey, CancellationToken.None)).ReturnsAsync(gameDto);

        // Act
        var result = await _controller.GetGameByKey(gameKey, CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.Equal(gameDto, okResult.Value);
    }

    [Fact]
    public async Task GetGenreByGameKeyAsync_ReturnGenres()
    {
        // Arrange
        var key = "key";
        var genresDtos = new List<GenreDto> { new() { Name = "abc" }, new() { Name = "xd" } };
        _gameServiceMock.Setup(s => s.GetGenresByGameKeyAsync(key, CancellationToken.None)).ReturnsAsync(genresDtos);

        // Act
        var result = await _controller.GetGenresByGameKey(key, CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.Equal(genresDtos, okResult.Value);
    }

    [Fact]
    public async Task GetPlatformsByGameKeyAsync_ReturnPlatforms()
    {
        // Arrange
        var key = "key";
        var platformsDtos = new List<PlatformDto> { new() { Type = "abc" }, new() { Type = "xd" } };
        _gameServiceMock.Setup(s => s.GetPlatformsByGameKeyAsync(key, CancellationToken.None)).ReturnsAsync(platformsDtos);

        // Act
        var result = await _controller.GetPlatformsByGameKey(key, CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.Equal(platformsDtos, okResult.Value);
    }

    [Fact]
    public async Task DownloadGameAsync_ReturnsNotFound_WhenGameDoesNotExist()
    {
        // Arrange
        var gameKey = "non-existent-key";
        _gameServiceMock.Setup(s => s.GetGameByKeyAsync(gameKey, CancellationToken.None)).ReturnsAsync((GameDto)null!);

        // Act
        var result = await _controller.DownloadGame(gameKey, CancellationToken.None);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        Assert.Equal($"Game with key {gameKey} not found.", notFoundResult.Value);
    }

    [Fact]
    public async Task GetPublisherByGameKeyAsync_ReturnsPublisher()
    {
        // Arrange
        var key = "key";
        var publisherDto = new PublisherDto { CompanyName = "abc" };
        _gameServiceMock.Setup(s => s.GetPublisherByGameKeyAsync(key, CancellationToken.None)).ReturnsAsync(publisherDto);

        // Act
        var result = await _controller.GetPublisherByGameKey(key, CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.Equal(publisherDto, okResult.Value);
    }

    [Fact]
    public async Task DownloadGameAsync_ReturnsFileResult_WithContentType()
    {
        // Arrange
        var gameKey = "key";
        var gameDto = new GameDto { Key = gameKey, Name = "xD" };
        _gameServiceMock.Setup(s => s.GetGameByKeyAsync(gameKey, CancellationToken.None)).ReturnsAsync(gameDto);
        if (!Directory.Exists("games"))
        {
            Directory.CreateDirectory("games");
        }

        var a = File.Create($"games/Test Game_{DateTime.UtcNow:dd_MM_yyyy}.json");
        a.Close();
        _gameFileServiceMock.Setup(s => s.CreateGameFileAsync(gameDto)).ReturnsAsync($"games/Test Game_{DateTime.UtcNow:dd_MM_yyyy}.json");

        // Act
        var result = await _controller.DownloadGame(gameKey, CancellationToken.None);

        // Assert
        var fileResult = result as FileResult;
        Assert.Equal("application/octet-stream", fileResult.ContentType);
    }

    [Fact]
    public async Task UpdateGameAsync_ReturnsNoContent_WhenSuccessful()
    {
        // Arrange
        var gameDto = new GameDto { Key = "key" };
        var gameUpdateRequestDto = new GameUpdateRequestDto { Game = gameDto };
        _gameServiceMock.Setup(s => s.UpdateGameAsync(gameUpdateRequestDto, CancellationToken.None)).ReturnsAsync(gameDto);

        // Act
        var result = await _controller.UpdateGame(gameUpdateRequestDto, CancellationToken.None);

        // Assert
        var noContentResult = result as NoContentResult;
        Assert.NotNull(noContentResult);
        _gameServiceMock.Verify(s => s.UpdateGameAsync(gameUpdateRequestDto, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetGameByIdAsync_ReturnsGame_WhenSuccessful()
    {
        // Arrange
        var gameDto = new GameDto { Id = Guid.NewGuid() };
        _gameServiceMock.Setup(s => s.GetGameByIdAsync(gameDto.Id, CancellationToken.None)).ReturnsAsync(gameDto);

        // Act
        var result = await _controller.GetGameById(gameDto.Id, CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.Equal(gameDto, okResult.Value);
    }

    [Fact]
    public async Task GetGameByIdAsync_ReturnNotFound_WhenGameNotFound()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        _gameServiceMock.Setup(s => s.GetGameByIdAsync(gameId, CancellationToken.None)).ReturnsAsync((GameDto)null!);

        // Act
        var result = await _controller.GetGameById(gameId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var notFoundResult = result.Result as NotFoundObjectResult;
        Assert.Equal($"Game with ID {gameId} could not be found.", notFoundResult.Value);
    }

    [Fact]
    public async Task BuyGame_ReturnsOk_WhenGameIsAdded()
    {
        // Arrange
        var gameKey = "testGame";
        _orderServiceMock.Setup(s => s.AddGameToCartAsync(CustomerStub.CustomerId, gameKey, CancellationToken.None))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.BuyGame(gameKey, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    public void Dispose()
    {
        _memoryCache.Dispose();
        GC.SuppressFinalize(this);
    }
}