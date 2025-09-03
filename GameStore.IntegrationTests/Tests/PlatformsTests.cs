using System.Net.Http.Json;
using GameStore.BLL.DTOs.Games;
using GameStore.BLL.DTOs.Platforms;
using GameStore.Data.Data;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.IntegrationTests.Helpers;
using GameStore.IntegrationTests.Setup;
using Microsoft.EntityFrameworkCore;

namespace GameStore.IntegrationTests.Tests;

public class PlatformsTests : IClassFixture<DatabaseFixture>
{
    private readonly HttpClient _client;

    private readonly GameStoreContext _context;

    public PlatformsTests(DatabaseFixture dbFixture)
    {
        var factory = new GameStoreWebApplicationFactory(dbFixture);
        _client = factory.CreateClient();
        _context = dbFixture.GameStoreContext;
        Authenticator.AuthenticateClientAsync(_client, factory.Services).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task CreatePlatform_CreatesPlatform()
    {
        // Arrange
        var request = new PlatformCreateRequest()
        {
            Platform = new PlatformCreateDto()
            {
                Type = "Giga platform",
            }
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/platforms", request);
        
        // Assert
        response.EnsureSuccessStatusCode();
        var platform = await response.Content.ReadFromJsonAsync<PlatformDto>();
        Assert.NotNull(platform);
        Assert.Equal(platform.Type, request.Platform.Type);
    }

    [Fact]
    public async Task DeletePlatform_DeletesPlatform()
    {
        // Arrange
        var platform = new Platform()
        {
            Type = "best platform",
            Id = Guid.NewGuid(),
        };
        _context.Platforms.Add(platform);
        await _context.SaveChangesAsync();
        
        // Act
        var response = await _client.DeleteAsync($"/platforms/{platform.Id}");
        
        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Null(await _context.Platforms
            .FirstOrDefaultAsync(p => p.Id == platform.Id));
    }

    [Fact]
    public async Task GetAllPlatforms_ReturnsPlatforms()
    {
        // Arrange
        var platform = new Platform()
        {
            Type = "getallplatforms",
            Id = Guid.NewGuid(),
        };
        _context.Platforms.Add(platform);
        await _context.SaveChangesAsync();
        
        // Act
        var response = await _client.GetAsync($"/platforms");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var platforms = await response.Content.ReadFromJsonAsync<List<PlatformDto>>();
        Assert.NotNull(platforms);
        Assert.NotEmpty(platforms);
    }

    [Fact]
    public async Task GetPlatformById_ReturnsPlatform()
    {
        // Arrange
        var platform = new Platform()
        {
            Type = "getplatformbyid",
            Id = Guid.NewGuid(),
        };
        _context.Platforms.Add(platform);
        await _context.SaveChangesAsync();
        
        // Act
        var response = await _client.GetAsync($"/platforms/{platform.Id}");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var platformFound = await response.Content.ReadFromJsonAsync<PlatformDto>();
        Assert.NotNull(platformFound);
        Assert.Equal(platform.Type, platformFound.Type);
    }

    [Fact]
    public async Task UpdatePlatform_UpdatesPlatform()
    {
        // Arrange
        var platform = new Platform()
        {
            Type = "updateplatform",
            Id = Guid.NewGuid(),
        };
        _context.Platforms.Add(platform);
        await _context.SaveChangesAsync();

        var platformRequest = new PlatformRequest()
        {
            Platform = new PlatformDto()
            {
                Id = platform.Id,
                Type = "Updated platform",
            }
        };
        
        // Act
        var response = await _client.PutAsJsonAsync($"/platforms", platformRequest);
        
        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(await _context.Platforms.FirstOrDefaultAsync(p => p.Type == platformRequest.Platform.Type));
    }

    [Fact]
    public async Task GetGamesByPlatformId_ReturnsGames()
    {
        // Arrange
        var platform = new Platform()
        {
            Type = "getgamesbyplatformid",
            Id = Guid.NewGuid(),
            Games = new List<Game>()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Key = "test",
                    Name = "test",
                    Publisher = new Publisher
                    {
                        Description = "test",
                        CompanyName = "testtest",
                        Id = Guid.NewGuid(),
                    }
                }
            }
        };
        _context.Platforms.Add(platform);
        await _context.SaveChangesAsync();
        
        // Act
        var response = await _client.GetAsync($"/platforms/{platform.Id}/games");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var games = await response.Content.ReadFromJsonAsync<List<GameDto>>();
        Assert.NotNull(games);
        Assert.NotEmpty(games);
    }
}