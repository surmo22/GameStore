using GameStore.BLL.Interfaces.Repositories;
using GameStore.Data.Data;
using GameStore.Data.Interfaces;
using GameStore.MongoData.Interfaces;
using GameStore.MongoData.Interfaces.GameDependencyResolvers;
using GameStore.MongoData.Interfaces.MongoRepositories;
using GameStore.MongoData.MongoRepositories;
using GameStore.MongoData.MongoRepositories.GameDependencyResolvers;
using GameStore.MongoData.SharedRepositories;
using GameStore.MongoData.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace GameStore.MongoData;

public static class MongoDataExtensions
{
    public static IServiceCollection AddMongoDataAccessServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IShippersRepository, ShippersRepository>();
        services.AddSingleton<IMongoClient>(
            new MongoClient(configuration.GetConnectionString("MongoDbConnectionString")));
        services.AddScoped<NorthwindMongoContext>();
        services.AddScoped<IMongoGameRepository, MongoGameRepository>();
        services.AddScoped<IMongoGenreRepository, MongoGenreRepository>();
        services.AddScoped<IMongoPublisherRepository, MongoPublisherRepository>();
        services.AddScoped<IMongoOrderRepository, MongoOrderRepository>();
        services.AddScoped<IMongoLogger, MongoLogger>();
        services.AddScoped<IDatabaseMigrator, DatabaseMigrator>();
        
        services.AddDecorators();

        services.AddMongoGameDependencyResolvers();
        return services;
    }

    private static void AddDecorators(this IServiceCollection services)
    {
        // decorators
        services.Decorate<IGameRepository, GameRepository>();
        services.Decorate<IPublisherRepository, PublisherRepository>();
        services.Decorate<IGenreRepository, GenreRepository>();
        services.Decorate<IOrderRepository, OrderRepository>();
        services.Decorate<ICommentRepository, CommentRepository>();
    }

    private static void AddMongoGameDependencyResolvers(this IServiceCollection services)
    {
        services.AddScoped<IMongoGameDependencyResolver, GenreResolver>();
        services.AddScoped<IMongoGameDependencyResolver, PublisherResolver>();
        services.AddScoped<IMongoGameDependenciesResolver, MongoGameDependenciesResolver>();
    }
}