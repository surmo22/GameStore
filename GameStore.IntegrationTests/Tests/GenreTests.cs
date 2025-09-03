using System.Net.Http.Json;
using GameStore.BLL.DTOs.Genres;
using GameStore.Common.Utils;
using GameStore.Data.Data;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.IntegrationTests.Helpers;
using GameStore.IntegrationTests.Setup;
using MongoDB.Driver;

namespace GameStore.IntegrationTests.Tests;

public class GenreTests : IClassFixture<DatabaseFixture>
{
    private readonly HttpClient _client;
    
    private readonly NorthwindMongoContext _mongoDb;
    
    private readonly GameStoreContext _context;

    public GenreTests(DatabaseFixture dbFixture)
    {
        var factory = new GameStoreWebApplicationFactory(dbFixture);
        _client = factory.CreateClient();
        _mongoDb = dbFixture.NorthwindContext;
        _context = dbFixture.GameStoreContext;
        Authenticator.AuthenticateClientAsync(_client, factory.Services).GetAwaiter().GetResult();
    }
    
    [Fact]
    public async Task GetGenres_ReturnsSeededSqlGenres()
    {
        // Arrange
        var genreToVerify = new Genre()
        {
            Id = Guid.NewGuid(),
            Name = "testgetallgenres",
        };
        _context.Genres.Add(genreToVerify);
        await _context.SaveChangesAsync();
        
        // Act
        var response = await _client.GetAsync("/genres");
        response.EnsureSuccessStatusCode();

        // Assert
        var genres = await response.Content.ReadFromJsonAsync<List<GenreDto>>();
        Assert.NotNull(genres);
        Assert.Contains(genres, g => g.Name == genreToVerify.Name);
    }
    
    [Fact]
    public async Task GetGenres_ReturnsSeededMongoGenres()
    {
        
        var response = await _client.GetAsync("/genres");
        response.EnsureSuccessStatusCode();

        var genres = await response.Content.ReadFromJsonAsync<List<GenreDto>>();
        Assert.NotNull(genres);
        Assert.Contains(genres, g => g.Name == "Fantasy");
    }

    [Fact]
    public async Task CreateGenre_CreatesNewGenre()
    {
        // Arrange
        var createGenreDto = new GenreCreateRequest
        {
            Genre = new GenreCreateDto()
            {
                Name = "TestGenre",
                ParentGenreId = null,
            },
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/genres", createGenreDto);
        
        // Assert
        response.EnsureSuccessStatusCode();
        var createdGenre = await response.Content.ReadFromJsonAsync<GenreDto>();
        Assert.NotNull(createdGenre);
        Assert.Equal(createGenreDto.Genre.Name, createdGenre.Name);
        Assert.NotNull(_context.Genres.FirstOrDefault(g => g.Name == "TestGenre"));
    }

    [Fact]
    public async Task DeleteGenre_DeletesExistingGenre()
    {
        // Arrange
        var genreToDelete = new Genre()
        {
            Id = Guid.NewGuid(),
            Name = "testdeletegenre",
        };
        _context.Genres.Add(genreToDelete);
        await _context.SaveChangesAsync();
        
        // Act
        var response = await _client.DeleteAsync($"/genres/{genreToDelete.Id}");
        
        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(_context.Genres.FirstOrDefault(g => g.Name == genreToDelete.Name && g.IsDeleted));
    }

    [Fact]
    public async Task UpdateGenre_UpdatesExistingGenre()
    {
        // Arrange
        var genreToUpdate = _context.Genres.FirstOrDefault(g => !g.IsDeleted);
        var updateRequest = new GenreRequest()
        {
            Genre = new GenreDto()
            {
                Id = genreToUpdate.Id,
                Name = "Updated Genre",
            }
        };
        
        // Act
        var response = await _client.PutAsJsonAsync("/genres", updateRequest);
        
        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(_context.Genres.FirstOrDefault(g => g.Name == updateRequest.Genre.Name));
    }
    
    [Fact]
    public async Task UpdateGenre_UpdatesExistingGenreFromMongoDb()
    {
        // Arrange
        var genreToUpdate = _mongoDb.Genres.AsQueryable().FirstOrDefault();
        Assert.NotNull(genreToUpdate);
        var updateRequest = new GenreRequest()
        {
            Genre = new GenreDto()
            {
                Id = IntToGuidConverter.Convert(genreToUpdate.CategoryId),
                Name = "Updated Genre From MongoDb",
            }
        };
        
        // Act
        var response = await _client.PutAsJsonAsync("/genres", updateRequest);
        
        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(_context.Genres.FirstOrDefault(g => g.Name == updateRequest.Genre.Name));
    }

    [Fact]
    public async Task GetGenreById_ReturnsExistingGenre()
    {
        // Arrange
        var genreToSearch = _mongoDb.Genres.AsQueryable().FirstOrDefault();
        Assert.NotNull(genreToSearch);
        var id = IntToGuidConverter.Convert(genreToSearch.CategoryId);
        
        // Act
        var response = await _client.GetAsync($"/genres/{id}");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var genre = await response.Content.ReadFromJsonAsync<GenreDto>();
        Assert.NotNull(genre);
        Assert.Equal(genre.Name, genreToSearch.CategoryName);
    }

    [Fact]
    public async Task GetNestedGenres_ReturnsNestedGenres()
    {
        // Arrange
        var idToSearch = _context.Genres
            .FirstOrDefault(g => g.ParentGenreId != null && !g.IsDeleted)?.ParentGenreId;
        
        if (idToSearch == null || idToSearch == Guid.Empty)
        {
            Assert.Fail("Could not find suitable genre");
        }
        
        // Act
        var response = await _client.GetAsync($"/genres/{idToSearch}/genres");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var genres = await response.Content.ReadFromJsonAsync<List<GenreDto>>();
        Assert.NotNull(genres);
        Assert.True(genres.Count > 0);
    }

    [Fact]
    public async Task GetGamesByGenreId_ReturnsGamesByGenre()
    {
        // Arrange
        var genre = _context.Genres.FirstOrDefault();
        if (genre is null)
        {
            Assert.Fail("Could not find suitable genre");
        }
        
        var game = new Game()
        {
            Id = Guid.NewGuid(),
            Name = "TestGame",
            Key = "keykekykey",
            Discount = 0,
            UnitsOnOrder = 2,
            Publisher = new Publisher()
            {
                CompanyName = "test",
                Id = Guid.NewGuid(),
            },
            Genres = [genre],
        };
        await _context.Games.AddAsync(game);
        await _context.SaveChangesAsync();
        
        // Act
        var response = await _client.GetAsync($"/genres/{genre.Id}/games");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var games = await response.Content.ReadFromJsonAsync<List<Game>>();
        Assert.NotNull(games);
        Assert.True(games.Count > 0);
    }
}