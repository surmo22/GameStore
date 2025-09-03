using System.Net.Http.Json;
using GameStore.Data.Data;
using GameStore.Domain.MongoEntities;
using GameStore.IntegrationTests.Helpers;
using GameStore.IntegrationTests.Setup;

namespace GameStore.IntegrationTests.Tests;

public class ShippersTests : IClassFixture<DatabaseFixture>
{
    private readonly HttpClient _client;
    
    private readonly NorthwindMongoContext _mongoContext;

    public ShippersTests(DatabaseFixture fixture)
    {
        var factory = new GameStoreWebApplicationFactory(fixture);
        _client = factory.CreateClient();
        _mongoContext = fixture.NorthwindContext;
        Authenticator.AuthenticateClientAsync(_client, factory.Services).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task GetAllShippers_ReturnsAllShippers()
    {
        // Arrange
        await _mongoContext.Shippers.InsertManyAsync([
            new MongoShipper()
            {
                ShipperId = 1,
            },
            new MongoShipper()
            {
                ShipperId = 2,
            }
        ]);
        
        // Act
        var response = await _client.GetAsync("Shippers");
        
        // Arrange
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<List<MongoShipper>>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }
}