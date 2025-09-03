using GameStore.BLL.Interfaces.ExternalServices;
using GameStore.BLL.Interfaces.NotificationServices;
using GameStore.Data.Data;
using GameStore.IntegrationTests.Setup.Fakes;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace GameStore.IntegrationTests.Setup;

internal sealed class GameStoreWebApplicationFactory(DatabaseFixture dbFixture) : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            AddSqlDb(services);

            AddMongoDb(services);
            
            AddFakeServices(services);
        });
        
        return base.CreateHost(builder);
    }

    private static void AddFakeServices(IServiceCollection services)
    {
        var paymentDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IPaymentExternalService));
        if (paymentDescriptor != null) services.Remove(paymentDescriptor);
        
        services.AddScoped<IPaymentExternalService, FakePaymentExternalService>();
        
        var authDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IAuthentificationExternalService));
        if (authDescriptor != null) services.Remove(authDescriptor);
        
        services.AddScoped<IAuthentificationExternalService, FakeExternalAuthService>();
        
        
        var azureStorageDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IImageStorageService));
        if (azureStorageDescriptor != null) services.Remove(azureStorageDescriptor);
        services.AddScoped<IImageStorageService, FakeAzureImageStorageService>();

        var azureServiceBusDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IAzureServiceBusService));
        if (azureServiceBusDescriptor != null) services.Remove(azureServiceBusDescriptor);
        services.AddScoped<IAzureServiceBusService, FakeAzureServiceBusService>();
    }

    private void AddMongoDb(IServiceCollection services)
    {
        var mongoDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IMongoClient));
        if (mongoDescriptor != null) services.Remove(mongoDescriptor);

        services.AddSingleton<IMongoClient>(_ => new MongoClient(dbFixture.MongoConnectionString));
    }

    private void AddSqlDb(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<GameStoreContext>));
        if (descriptor != null) services.Remove(descriptor);
            
        services.AddDbContext<GameStoreContext>(options =>
            options.UseSqlServer(dbFixture.SqlConnectionString));
    }
}