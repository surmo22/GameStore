using System.Net.Http.Json;
using GameStore.Data.Data;
using GameStore.IntegrationTests.Helpers;
using GameStore.IntegrationTests.Setup;
using Microsoft.EntityFrameworkCore;

namespace GameStore.IntegrationTests.Tests;

public class CommentsTests : IClassFixture<DatabaseFixture>
{
    private readonly HttpClient _client;
    private readonly GameStoreContext _context;

    public CommentsTests(DatabaseFixture dbFixture)
    {
        var factory = new GameStoreWebApplicationFactory(dbFixture);
        _client = factory.CreateClient();
        _context = dbFixture.GameStoreContext;
        Authenticator.AuthenticateClientAsync(_client, factory.Services).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task GetBanDuration_ReturnsOptions()
    {
        // Act
        var response = await _client.GetAsync("/comments/ban/durations");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<List<string>>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public async Task BanUser_BansUser()
    {
        // Arrange
        var request = new
        {
            User = "gigatest",
            duration = "One Day",
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/comments/ban/", request);
        
        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(await _context.UserBans
            .Include(ub => ub.User)
            .FirstOrDefaultAsync(ub => ub.User.UserName == request.User));
    }
}