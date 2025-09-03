using System.Net.Http.Json;
using GameStore.BLL.DTOs.Games;
using GameStore.BLL.DTOs.Publisher;
using GameStore.Data.Data;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.IntegrationTests.Helpers;
using GameStore.IntegrationTests.Setup;

namespace GameStore.IntegrationTests.Tests;

public class PublisherTests: IClassFixture<DatabaseFixture>
{
    private readonly HttpClient _client;
    
    private readonly GameStoreContext _context;

    public PublisherTests(DatabaseFixture fixture)
    {
        var factory = new GameStoreWebApplicationFactory(fixture);
        _client = factory.CreateClient();
        _context = fixture.GameStoreContext;
        Authenticator.AuthenticateClientAsync(_client, factory.Services).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task CreatePublisher_CreatesPublisher()
    {
        // Arrange
        var createPublisherRequest = new PublisherCreateRequest()
        {
            Publisher = new PublisherCreateDto
            {
                CompanyName = "Super publisher 4A games only",
                HomePage = "https://www.superpublisher.com",
                Description = "Super publisher 4A games only",
            }
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/publishers", createPublisherRequest);
        
        // Assert
        response.EnsureSuccessStatusCode();
        var publisher = await response.Content.ReadFromJsonAsync<PublisherDto>();
        Assert.NotNull(publisher);
        Assert.Equal(createPublisherRequest.Publisher.CompanyName, publisher.CompanyName);
        Assert.NotNull(_context.Publishers
            .FirstOrDefault(p => p.CompanyName == createPublisherRequest.Publisher.CompanyName));
    }

    [Fact]
    public async Task DeletePublisher_DeletesPublisher()
    {
        // Arrange
        var publisher = new Publisher()
        {
            Id = Guid.NewGuid(),
            CompanyName = "Super puper",
        };
        await _context.Publishers.AddAsync(publisher);
        await _context.SaveChangesAsync();
        
        // Act
        var response = await _client.DeleteAsync($"/publishers/{publisher.Id}");
        
        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(_context.Publishers
            .FirstOrDefault(p => p.CompanyName == publisher.CompanyName && p.IsDeleted));
    }

    [Fact]
    public async Task GetAllPublishers_ReturnsPublishers()
    {
        // Arrange
        // Act
        var response = await _client.GetAsync("/publishers");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var publishers = await response.Content.ReadFromJsonAsync<List<PublisherDto>>();
        Assert.NotNull(publishers);
        Assert.NotEmpty(publishers);
    }

    [Fact]
    public async Task GetPublisherByCompanyName_ReturnsPublisher()
    {
        // Arrange
        var publisher = new Publisher()
        {
            Id = Guid.NewGuid(),
            CompanyName = "GetPublisherByCompanyName",
            Description = "Super publisher 4A games only",
        };
        await _context.Publishers.AddAsync(publisher);
        await _context.SaveChangesAsync();
        
        // Act
        var response = await _client.GetAsync($"/publishers/{publisher.CompanyName}");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var publishers = await response.Content.ReadFromJsonAsync<PublisherDto>();
        Assert.NotNull(publishers);
        Assert.Equal(publisher.CompanyName, publisher.CompanyName);
    }

    [Fact]
    public async Task UpdatePublisher_UpdatesPublisher()
    {
        // Arrange
        var publisher = new Publisher()
        {
            Id = Guid.NewGuid(),
            CompanyName = "UpdateTest",
            Description = "Super publisher 4A games only",
        };
        await _context.Publishers.AddAsync(publisher);
        await _context.SaveChangesAsync();
        var publisherUpdateRequest = new PublisherRequest()
        {
            Publisher = new PublisherDto()
            {
                Id = publisher.Id,
                CompanyName = "UpdatedName",
                Description = "Super publisher 4A games only",
            },
        };
        
        // Act
        var response = await _client.PutAsJsonAsync($"/publishers/", publisherUpdateRequest);
        
        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(_context.Publishers.FirstOrDefault(p => p.CompanyName == publisherUpdateRequest.Publisher.CompanyName));
    }

    [Fact]
    public async Task GetGamesByPublisherId_ReturnsGames()
    {
        // Arrange
        var publisher = new Publisher()
        {
            Id = Guid.NewGuid(),
            CompanyName = "GetGamesByPublisherId",
            Description = "Super publisher 4A games only",
            Games = new List<Game>()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Key = "testtestovich",
                    Price = 1,
                    UnitInStock = 1,
                    Name = "testtestovich",
                }
            }
        };
        await _context.Publishers.AddAsync(publisher);
        await _context.SaveChangesAsync();
        
        // Act
        var response = await _client.GetAsync($"/publishers/{publisher.CompanyName}/games");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var games = await response.Content.ReadFromJsonAsync<List<GameDto>>();
        Assert.NotNull(games);
        Assert.NotEmpty(games);
    }
}