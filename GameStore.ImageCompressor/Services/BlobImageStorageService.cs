using Azure.Storage.Blobs;
using GameStore.ImageCompressor.Interfaces;
using GameStore.ImageCompressor.Utils;
using Microsoft.Extensions.Options;

namespace GameStore.ImageCompressor.Services;

public class BlobImageStorageService(
    BlobServiceClient blobServiceClient,
    IOptions<ImageCompressorOptions> options)
    : IImageStorageService
{
    private readonly ImageCompressorOptions _options = options.Value;

    public async Task<byte[]> DownloadImageAsync(string filename, CancellationToken cancellationToken)
    {
        var container = blobServiceClient.GetBlobContainerClient(_options.BlobContainerName);
        var blob = container.GetBlobClient(filename);
        var content = await blob.DownloadContentAsync(cancellationToken);
        return content.Value.Content.ToArray();
    }

    public async Task UploadImageAsync(string filename, Stream imageStream, CancellationToken cancellationToken)
    {
        var container = blobServiceClient.GetBlobContainerClient(_options.BlobContainerName);
        var blob = container.GetBlobClient(filename);
        await blob.UploadAsync(imageStream, overwrite: true, cancellationToken);
    }
}