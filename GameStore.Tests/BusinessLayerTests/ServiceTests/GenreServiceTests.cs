using AutoMapper;
using GameStore.BLL.DTOs.Games;
using GameStore.BLL.DTOs.Genres;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Interfaces.Repositories;
using GameStore.BLL.Services.EntityServices;
using GameStore.Common.Exceptions;
using GameStore.Domain.Entities.CoreEntities;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests;

public class GenreServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGenreRepository> _genreRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IGuidProvider> _guidProvider = new();
    private readonly GenreService _genreService;

    public GenreServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _genreRepositoryMock = new Mock<IGenreRepository>();
        _mapperMock = new Mock<IMapper>();
        _guidProvider.Setup(g => g.NewGuid()).Returns(Guid.NewGuid());
        

        _unitOfWorkMock.Setup(uow => uow.Genres).Returns(_genreRepositoryMock.Object);

        _genreService = new GenreService(_guidProvider.Object, _unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreateGenreAsync_ShouldCreateAndReturnGenre()
    {
        // Arrange
        var genreCreateDto = new GenreCreateDto { Name = "New Genre" };
        var genre = new Genre { Id = Guid.NewGuid(), Name = "New Genre" };
        var genreDto = new GenreDto { Id = genre.Id, Name = genre.Name };

        _mapperMock.Setup(m => m.Map<Genre>(genreCreateDto)).Returns(genre);
        _mapperMock.Setup(m => m.Map<GenreDto>(genre)).Returns(genreDto);
        _genreRepositoryMock.Setup(repo => repo.AddGenreAsync(genre, CancellationToken.None)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.CompleteAsync(CancellationToken.None)).Returns(Task.CompletedTask);

        // Act
        var result = await _genreService.CreateGenreAsync(genreCreateDto, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(genreDto.Name, result.Name);
        _genreRepositoryMock.Verify(repo => repo.AddGenreAsync(genre, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task DeleteGenreAsync_ShouldDeleteGenre()
    {
        // Arrange
        var genreId = Guid.NewGuid();
        _unitOfWorkMock.Setup(x => x.Genres.GenreExistsAsync(genreId, CancellationToken.None)).ReturnsAsync(true);

        // Act
        await _genreService.DeleteGenreAsync(genreId, CancellationToken.None);

        // Assert
        _genreRepositoryMock.Verify(repo => repo.DeleteGenreAsync(genreId, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetAllGenresAsync_ShouldReturnAllGenres()
    {
        // Arrange
        var genres = new List<Genre>
        {
            new() { Id = Guid.NewGuid(), Name = "Genre 1" },
            new() { Id = Guid.NewGuid(), Name = "Genre 2" },
        };
        var genreDtos = new List<GenreDto>
        {
            new() { Id = genres[0].Id, Name = genres[0].Name },
            new() { Id = genres[1].Id, Name = genres[1].Name },
        };

        _genreRepositoryMock.Setup(repo => repo.GetAllGenresAsync(CancellationToken.None)).ReturnsAsync(genres);
        _mapperMock.Setup(m => m.Map<IEnumerable<GenreDto>>(genres)).Returns(genreDtos);

        // Act
        var result = await _genreService.GetAllGenresAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(genreDtos, result);
    }

    [Fact]
    public async Task GetGenreByIdAsync_ShouldReturnGenre()
    {
        // Arrange
        var genreId = Guid.NewGuid();
        var genre = new Genre { Id = genreId, Name = "Genre" };
        var genreDto = new GenreDto { Id = genreId, Name = "Genre" };

        _genreRepositoryMock.Setup(repo => repo.GetGenreByIdAsync(genreId, CancellationToken.None)).ReturnsAsync(genre);
        _mapperMock.Setup(m => m.Map<GenreDto>(genre)).Returns(genreDto);

        // Act
        var result = await _genreService.GetGenreByIdAsync(genreId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(genreDto.Id, result.Id);
        Assert.Equal(genreDto.Name, result.Name);
    }

    [Fact]
    public async Task UpdateGenreAsync_ShouldUpdateGenre()
    {
        // Arrange
        var genreDto = new GenreDto { Id = Guid.NewGuid(), Name = "Updated Genre" };
        var genre = new Genre { Id = genreDto.Id, Name = "Updated Genre" };

        _mapperMock.Setup(m => m.Map<Genre>(genreDto)).Returns(genre);
        _genreRepositoryMock.Setup(repo => repo.UpdateGenre(genre, CancellationToken.None));
        _unitOfWorkMock.Setup(uow => uow.CompleteAsync(CancellationToken.None)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.Genres.GenreExistsAsync(genre.Id, CancellationToken.None)).ReturnsAsync(true);

        // Act
        await _genreService.UpdateGenreAsync(genreDto, CancellationToken.None);

        // Assert
        _genreRepositoryMock.Verify(repo => repo.UpdateGenre(genre, CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdateGenreAsync_ShouldThrowInvalidOperationException_WhenGenreIdEqualsParentId()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var genreDto = new GenreDto { Id = guid, ParentGenreId = guid.ToString() };
        var genre = new Genre { Id = guid, ParentGenreId = guid };

        _mapperMock.Setup(m => m.Map<Genre>(genreDto)).Returns(genre);
        _unitOfWorkMock.Setup(x => x.Genres.GenreExistsAsync(guid, CancellationToken.None)).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidGenreHierarchyException>(async () =>
            await _genreService.UpdateGenreAsync(genreDto, CancellationToken.None));
    }

    [Fact]
    public async Task GetGamesByGenreIdAsync_ShouldReturnGames()
    {
        // Arrange
        var genreId = Guid.NewGuid();
        var gameList = new List<Game> { new() { Id = Guid.NewGuid(), Name = "Game 1" } };
        var genre = new Genre { Id = genreId, Games = gameList };
        var gameDtos = new List<GameDto> { new() { Id = genre.Games.First().Id, Name = "Game 1" } };

        _genreRepositoryMock.Setup(repo => repo.GetGenreByIdAsync(genreId, CancellationToken.None)).ReturnsAsync(genre);
        _mapperMock.Setup(m => m.Map<IEnumerable<GameDto>>(genre.Games)).Returns(gameDtos);

        // Act
        var result = await _genreService.GetGamesByGenreIdAsync(genreId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(gameDtos, result);
    }

    [Fact]
    public async Task GetGamesByGenreIdAsync_ShouldThrowArgumentException_WhenGenreNotFound()
    {
        // Arrange
        var genreId = Guid.NewGuid();
        _genreRepositoryMock.Setup(repo => repo.GetGenreByIdAsync(genreId, CancellationToken.None)).ReturnsAsync((Genre)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _genreService.GetGamesByGenreIdAsync(genreId, CancellationToken.None));

        Assert.Contains($"{genreId}", exception.Message);
    }

    [Fact]
    public async Task UpdateGenreAsync_ShouldThrowKeyNotFoundException_WhenGenreNotExist()
    {
        // Arrange
        var genreDto = new GenreDto { Id = Guid.NewGuid(), Name = "Updated Genre" };
        var genre = new Genre { Id = genreDto.Id, Name = "Updated Genre" };

        _mapperMock.Setup(m => m.Map<Genre>(genreDto)).Returns(genre);
        _genreRepositoryMock.Setup(repo => repo.UpdateGenre(genre, CancellationToken.None));
        _unitOfWorkMock.Setup(uow => uow.CompleteAsync(CancellationToken.None)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.Genres.GenreExistsAsync(genre.Id, CancellationToken.None)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _genreService.UpdateGenreAsync(genreDto, CancellationToken.None));
    }

    [Fact]
    public async Task GetGenresByIdsAsync_ReturnsGenres_WhenIdsAreValid()
    {
        // Arrange
        var genreIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var genres = new List<Genre>
        {
            new() { Id = genreIds[0], Name = "Genre1" },
            new() { Id = genreIds[1], Name = "Genre2" },
        };

        _unitOfWorkMock.Setup(uow => uow.Genres.GetGenreByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(new Func<Guid, CancellationToken, Genre>((id, _) => genres.Find(g => g.Id == id)));

        // Act
        var result = await _genreService.GetGenresByIdsAsync(genreIds, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.NotNull(result.FirstOrDefault(g => g.Id == genreIds[0]));
        Assert.NotNull(result.FirstOrDefault(g => g.Id == genreIds[1]));
    }

    [Fact]
    public async Task GetGenresByIdsAsync_ThrowsKeyNotFoundException_WhenAGenreIdIsNotFound()
    {
        // Arrange
        var genreIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        _unitOfWorkMock.Setup(uow => uow.Genres.GetGenreByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync((Genre)null!);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _genreService.GetGenresByIdsAsync(genreIds, CancellationToken.None));
    }

    [Fact]
    public async Task GetGenresByIdsAsync_ReturnsEmptyList_WhenIdsAreEmpty()
    {
        // Arrange
        var genreIds = new List<Guid>();

        // Act
        var result = await _genreService.GetGenresByIdsAsync(genreIds, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetNestedGenresAsync_ShouldReturnNestedGenres()
    {
        // Arrange
        var genreId = Guid.NewGuid();
        var genre = new Genre { Id = genreId, Name = "Genre" };
        var nestedGenres = new List<Genre>
        {
            new() { Id = Guid.NewGuid(), Name = "Nested Genre 1" },
            new() { Id = Guid.NewGuid(), Name = "Nested Genre 2" },
        };
        var nestedGenreDtos = new List<GenreDto>
        {
            new() { Id = nestedGenres[0].Id, Name = nestedGenres[0].Name },
            new() { Id = nestedGenres[1].Id, Name = nestedGenres[1].Name },
        };

        _genreRepositoryMock.Setup(repo => repo.GetGenreByIdAsync(genreId, CancellationToken.None)).ReturnsAsync(genre);
        _mapperMock.Setup(m => m.Map<IEnumerable<GenreDto>>(genre.SubGenres)).Returns(nestedGenreDtos);
        _genreRepositoryMock.Setup(repo => repo.GenreExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _genreService.GetNestedGenresAsync(genreId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(nestedGenreDtos, result);
    }
}
