using GameStore.Data.Data;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Testcontainers.MongoDb;
using Testcontainers.MsSql;

namespace GameStore.IntegrationTests.Setup;

public class DatabaseFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("SuperSecretPassword!!")
        .Build();

    private readonly MongoDbContainer _mongoContainer = new MongoDbBuilder()
        .WithImage("mongo:latest")
        .Build();

    public string SqlConnectionString => _sqlContainer.GetConnectionString();
    public string MongoConnectionString => _mongoContainer.GetConnectionString();

    public NorthwindMongoContext NorthwindContext { get; private set; }
    
    public GameStoreContext GameStoreContext { get; private set; } = null!;
    
    private IMongoDatabase MongoDatabase { get; set; } = null!;

    public async Task InitializeAsync()
    {
        await InitSqlDbAsync();

        await InitMongoDbAsync();
    }
    
    private async Task InitMongoDbAsync()
    {
        await _mongoContainer.StartAsync();
        var mongoClient = new MongoClient(MongoConnectionString);
        NorthwindContext = new NorthwindMongoContext(mongoClient);
        MongoDatabase = mongoClient.GetDatabase("Northwind");
        
        await SeedMongoDatabase();
    }

    private async Task InitSqlDbAsync()
    {
        await _sqlContainer.StartAsync();
        GameStoreContext = new GameStoreContext(new DbContextOptionsBuilder<GameStoreContext>()
            .UseSqlServer(SqlConnectionString).Options);
        
        await GameStoreContext.Database.MigrateAsync();
        
        await SeedPublishers();
        
        await SeedGames();
        
        await GameStoreContext.SaveChangesAsync();
    }
    
    private async Task SeedGames()
    {
        await GameStoreContext.Games.AddAsync(new Game()
        {
            Id = Guid.NewGuid(),
            Name = "Super Game",
            Key = "supergame",
            Price = 11,
            PublisherId = Guid.Parse("3EE3AEE3-3E8B-4B63-BF35-CD43BE25BA8F"),
        });
    }

    private async Task SeedPublishers()
    {
        await GameStoreContext.Publishers.AddAsync(new Publisher
        {
            Id = Guid.Parse("3EE3AEE3-3E8B-4B63-BF35-CD43BE25BA8F"),
            CompanyName = "testtestovich",
        });
    }

    private async Task SeedMongoDatabase()
    {
        var genreCollection = MongoDatabase.GetCollection<MongoGenre>("categories");

        await genreCollection.InsertManyAsync([
            new MongoGenre { CategoryId = 1, CategoryName = "Sci-Fi" },
            new MongoGenre { CategoryId = 2, CategoryName = "Fantasy" }
        ]);
        
        var gamesCollection = MongoDatabase.GetCollection<MongoGame>("products");

        await gamesCollection.InsertManyAsync([
            new MongoGame
            {
                CategoryId = 1, ProductId = 1, ProductKey = "game1", ProductName = "test", UnitPrice = 12,
                UnitsInStock = 12,
            },
            new MongoGame
            {
                CategoryId = 2, ProductId = 2, ProductKey = "game2", ProductName = "test1", UnitPrice = 12,
                UnitsInStock = 12,
            }
        ]);
    }

    public async Task DisposeAsync()
    {
        await _sqlContainer.DisposeAsync();
        await _mongoContainer.DisposeAsync();
    }
}