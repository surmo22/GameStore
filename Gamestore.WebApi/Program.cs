using System.Runtime.CompilerServices;
using GameStore.AzureServiceBus;
using GameStore.AzureStorage;
using GameStore.BLL;
using GameStore.Data;
using GameStore.MongoData;
using GameStore.WebApi.Configurations;
using Serilog;

[assembly: InternalsVisibleTo("GameStore.IntegrationTests")]
var builder = WebApplication.CreateBuilder(args);
try
{
    builder.SetupSerilog();
    builder.Services.AddApiServices(builder.Configuration)
        .AddDataAccessServices(builder.Configuration)
        .AddMongoDataAccessServices(builder.Configuration)
        .AddBusinessServices()
        .AddGameCountService()
        .AddAzureStorage()
        .AddAzureServiceBus();

    var app = builder
        .Build();

    await app.UseApiConfigurations()
        .RunAsync();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Unhandled exception has occured");
    throw;
}
finally
{
    await Log.CloseAndFlushAsync();
}
