using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using GameStore.AzureStorage.Options;
using GameStore.AzureStorage.Services;
using GameStore.BLL.Interfaces.ExternalServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GameStore.AzureStorage;

public static class AzureStorageConfigurationExtensions
{
    public static IServiceCollection AddAzureStorage(this IServiceCollection services)
    {
        services.AddOptions<AzureStorageOptions>()
            .Configure<IConfiguration>((settings, config) =>
                config.GetSection("AzureStorage").Bind(settings));

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AzureStorageOptions>>().Value;
            return new BlobContainerClient(options.ConnectionString, options.BlobContainerName);
        });

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AzureStorageOptions>>().Value;
            return new QueueClient(options.ConnectionString, options.QueueName, new QueueClientOptions()
            {
                MessageEncoding = QueueMessageEncoding.Base64
            });
        });
        
        services.AddScoped<IImageStorageService, AzureImageStorageService>();
        
        return services;
    }
}