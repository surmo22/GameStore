using Bogus;
using GameStore.BLL.Interfaces.GameServices;
using GameStore.BLL.Services.GameServices;
using GameStore.Data.Data;
using GameStore.Domain.Entities.CoreEntities;
using MongoDB.Driver;

namespace GameStore.WebApi.Configurations;

public static class StartUpTasks
{
    public static IServiceCollection AddGameCountService(this IServiceCollection services)
    {
        services.AddSingleton<IGameCountService, GameCountService>(serviceProvider =>
        {
            using var scope = serviceProvider.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();
            var mongoContext = scope.ServiceProvider.GetRequiredService<NorthwindMongoContext>();
            
            var sqlGameKeys = new HashSet<string>(dbContext.Games.Select(g => g.Key));
            
            var mongoGames = mongoContext.Games.AsQueryable().ToList();
            
            var mongoGamesCount = mongoGames.Count(game => !sqlGameKeys.Contains(game.ProductKey));

            var gamesCount = sqlGameKeys.Count + mongoGamesCount;

            return new GameCountService(gamesCount);
        });

        return services;
    }

    public static IServiceCollection SeedData(this IServiceCollection services)
    {
        using var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();
        if (dbContext.Games.Count() > 100000)
        {
            return services;
        }

        var publisherFaker = new Faker<Publisher>()
            .RuleFor(p => p.Id, _ => Guid.NewGuid())
            .RuleFor(p => p.CompanyName, f => f.Company.CompanyName());

        var genreFaker = new Faker<Genre>()
            .RuleFor(g => g.Id, _ => Guid.NewGuid())
            .RuleFor(g => g.Name, _ => $"Genre-{Guid.NewGuid().ToString("N")[..8]}");

        var platformFaker = new Faker<Platform>()
            .RuleFor(p => p.Id, _ => Guid.NewGuid())
            .RuleFor(p => p.Type, _ => $"Genre-{Guid.NewGuid().ToString("N")[..8]}");

        var publishers = publisherFaker.Generate(50).ToList();
        var genres = genreFaker.Generate(20).ToList();
        var platforms = platformFaker.Generate(10).ToList();

        dbContext.Publishers.AddRange(publishers);
        dbContext.Genres.AddRange(genres);
        dbContext.Platforms.AddRange(platforms);
        dbContext.SaveChanges();

        var gameFaker = new Faker<Game>()
            .RuleFor(g => g.Id, _ => Guid.NewGuid())
            .RuleFor(g => g.Name, f => f.Random.AlphaNumeric(20))
            .RuleFor(g => g.Key, f => f.Random.AlphaNumeric(10))
            .RuleFor(g => g.ImageUrl, f => f.Image.PicsumUrl())
            .RuleFor(g => g.Description, f => f.Lorem.Sentence())
            .RuleFor(g => g.Price, f => f.Random.Double(10, 100))
            .RuleFor(g => g.UnitInStock, f => f.Random.Int(1, 100))
            .RuleFor(g => g.Discount, f => f.Random.Int(0, 50))
            .RuleFor(g => g.CreationDate, f => f.Date.Past(5))
            .RuleFor(g => g.ViewCount, f => f.Random.Int(0, 10000))
            .RuleFor(g => g.IsDeleted, _ => false)
            .RuleFor(g => g.QuantityPerUnit, f => f.Commerce.ProductAdjective())
            .RuleFor(g => g.UnitsOnOrder, f => f.Random.Int(0, 20))
            .RuleFor(g => g.ReorderLevel, f => f.Random.Int(1, 5))
            .RuleFor(g => g.Discontinued, f => f.Random.Bool(0.1f))
            .RuleFor(g => g.PublisherId, f => f.PickRandom(publishers).Id)
            .RuleFor(g => g.Genres, f => f.PickRandom(genres, f.Random.Int(1, 3)).ToList())
            .RuleFor(g => g.Platforms, f => f.PickRandom(platforms, f.Random.Int(1, 2)).ToList());

        foreach (var _ in Enumerable.Range(0, 20))
        {
            var games = gameFaker.Generate(5000); // 20 * 5,000 = 100,000
            dbContext.Games.AddRange(games);
            dbContext.SaveChanges();
        }

        return services;
    }
}