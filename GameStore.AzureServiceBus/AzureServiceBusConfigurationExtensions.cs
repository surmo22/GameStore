using Azure.Messaging.ServiceBus;
using GameStore.AzureServiceBus.Services;
using GameStore.BLL.Interfaces.NotificationServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameStore.AzureServiceBus;

public static class AzureServiceBusConfigurationExtensions
{
    public static IServiceCollection AddAzureServiceBus(this IServiceCollection services)
    {
        services.AddSingleton<ServiceBusClient>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var connectionString = config.GetConnectionString("AzureServiceBus");
            return new ServiceBusClient(connectionString);
        });
        
        services.AddScoped<IAzureServiceBusService, AzureServiceBusService>();
        return services;
    }
}