using GameStore.BLL.Interfaces.Repositories;
using GameStore.Data.Data;
using GameStore.Data.Repositories;
using GameStore.Data.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameStore.Data;

public static class DataAccessLayerExtensions
{
    public static IServiceCollection AddDataAccessServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IGameRepository, SqlGameRepository>();
        services.AddScoped<IGenreRepository, SqlGenreRepository>();
        services.AddScoped<IPlatformRepository, PlatformRepository>();
        services.AddScoped<IPublisherRepository, SqlPublisherRepository>();
        services.AddScoped<IOrderRepository, SqlOrderRepository>();
        services.AddScoped<IPaymentMethodsRepository, PaymentMethodsRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IUserBanRepository, UserBanRepository>();
        services.AddScoped<ICoreRepositories, CoreRepositories>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddScoped<EntityChangeInterceptor>();
        services.AddDbContext<GameStoreContext>((serviceProvider, options) =>
        {
            options.EnableSensitiveDataLogging();
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            options.AddInterceptors(serviceProvider.GetService<EntityChangeInterceptor>()!);
        });
        
        return services;
    }
}