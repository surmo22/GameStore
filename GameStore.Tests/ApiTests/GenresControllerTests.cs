using GameStore.BLL.DTOs.Games;
using GameStore.BLL.DTOs.Genres;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Tests.ApiTests;

public class GenresControllerTests
{
    private readonly Mock<IGenreService> _genreServiceMock;
    private readonly GenresController _controller;

    public GenresControllerTests()
    {
        _genreServiceMock = new Mock<IGenreService>();
        _controller = new GenresController(_genreServiceMock.Object);
    }

    [Fact]
    public async Task CreateGenreAsync_ReturnsCreatedAtActionResult_WithGenreDto()
    {
        // Arrange
        var genreCreateDto = new GenreCreateDto { Name = "Test Genre" };
        var genreCreateRequest = new GenreCreateRequest()
        {
            Genre = genreCreateDto,
        };
        var genreDto = new GenreDto { Id = Guid.NewGuid(), Name = "Test Genre" };
        _genreServiceMock.Setup(s => s.CreateGenreAsync(genreCreateDto, CancellationToken.None))
                         .ReturnsAsync(genreDto);

        // Act
        var result = await _controller.CreateGenre(genreCreateRequest, CancellationToken.None);

        // Assert
        var createdAtActionResult = result.Result as CreatedAtActionResult;
        Assert.NotNull(createdAtActionResult);
        Assert.Equal("GetGenreById", createdAtActionResult.ActionName);
        Assert.Equal(genreDto.Id, createdAtActionResult.RouteValues["id"]);
        var returnedGenre = createdAtActionResult.Value as GenreDto;
        Assert.NotNull(returnedGenre);
        Assert.Equal(genreDto.Name, returnedGenre.Name);
    }

    [Fact]
    public async Task GetNestedGenres_ReturnsNestedGenres()
    {
        // Arrange
        var genreId = Guid.NewGuid();
        var genres = new List<GenreDto>()
        {
            new(),
            new(),
        };
        _genreServiceMock.Setup(s => s.GetNestedGenresAsync(genreId, CancellationToken.None)).ReturnsAsync(genres);

        // Act
        var result = await _controller.GetNestedGenres(genreId, CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        var returnedGenres = okResult.Value as IEnumerable<GenreDto>;
        Assert.NotNull(returnedGenres);
        Assert.Equal(genres, returnedGenres);
    }

    [Fact]
    public async Task DeleteGenreAsync_ReturnsNoContentResult()
    {
        // Arrange
        var genreId = Guid.NewGuid();
        _genreServiceMock.Setup(s => s.DeleteGenreAsync(genreId, CancellationToken.None)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteGenre(genreId, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task GetAllGenresAsync_ReturnsOkResult_WithGenres()
    {
        // Arrange
        var genres = new List<GenreDto>
            {
                new() { Id = Guid.NewGuid(), Name = "Genre 1" },
                new() { Id = Guid.NewGuid(), Name = "Genre 2" },
            };
        _genreServiceMock.Setup(s => s.GetAllGenresAsync(CancellationToken.None)).ReturnsAsync(genres);

        // Act
        var result = await _controller.GetAllGenres(CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        var returnedGenres = okResult.Value as IEnumerable<GenreDto>;
        Assert.NotNull(returnedGenres);
        Assert.Equal(2, returnedGenres.Count());
    }

    [Fact]
    public async Task GetGenreByIdAsync_ReturnsOkResult_WithGenreDto()
    {
        // Arrange
        var genreId = Guid.NewGuid();
        var genreDto = new GenreDto { Id = genreId, Name = "Test Genre" };
        _genreServiceMock.Setup(s => s.GetGenreByIdAsync(genreId, CancellationToken.None)).ReturnsAsync(genreDto);

        // Act
        var result = await _controller.GetGenreById(genreId, CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        var returnedGenre = okResult.Value as GenreDto;
        Assert.NotNull(returnedGenre);
        Assert.Equal(genreDto.Id, returnedGenre.Id);
    }

    [Fact]
    public async Task GetGenreByIdAsync_ReturnsNotFoundResult()
    {
        // Arrange
        var genreId = Guid.NewGuid();
        _genreServiceMock.Setup(s => s.GetGenreByIdAsync(genreId, CancellationToken.None)).ReturnsAsync((GenreDto)null!);

        // Act
        var result = await _controller.GetGenreById(genreId, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateGenreAsync_ReturnsNoContentResult()
    {
        // Arrange
        var genreDto = new GenreDto { Id = Guid.NewGuid(), Name = "Updated Genre" };
        var genreReqeust = new GenreRequest()
        {
            Genre = genreDto,
        };
        _genreServiceMock.Setup(s => s.UpdateGenreAsync(genreDto, CancellationToken.None)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateGenre(genreReqeust, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task GetGamesByGenreIdAsync_ReturnsGames()
    {
        // Arrange
        var genreId = Guid.NewGuid();
        var games = new List<GameDto>
        {
            new() { Name = "Game 1" },
            new() { Name = "Game 2" },
        };
        _genreServiceMock.Setup(s => s.GetGamesByGenreIdAsync(genreId, CancellationToken.None)).ReturnsAsync(games);

        // Act
        var result = await _controller.GetGamesByGenreId(genreId, CancellationToken.None);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        var returnedGames = okResult.Value as IEnumerable<GameDto>;
        Assert.NotNull(returnedGames);
        Assert.Equal(games, returnedGames);
    }
}