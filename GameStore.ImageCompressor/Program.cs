using Azure.Storage.Blobs;
using GameStore.ImageCompressor.Interfaces;
using GameStore.ImageCompressor.Services;
using GameStore.ImageCompressor.Utils;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.Configure<ImageCompressorOptions>(builder.Configuration.GetSection("ImageProcessing"));

builder.Services
    .AddSingleton(x =>
    {
        var config = x.GetRequiredService<IConfiguration>();
        var connectionString = config.GetValue<string>("AzureWebJobsStorage");
        return new BlobServiceClient(connectionString);
    });

builder.Services.AddSingleton<IImageCompressionService, ImageCompressionService>()
    .AddSingleton<IImageStorageService, BlobImageStorageService>()
    .AddSingleton<IImageEncoderFactory, ImageEncoderFactory>()
    .AddSingleton<IImageResizer, ImageResizer>();

builder.Build().Run();