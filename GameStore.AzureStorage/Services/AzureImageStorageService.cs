using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;
using GameStore.BLL.Interfaces.ExternalServices;

namespace GameStore.AzureStorage.Services;

public class AzureImageStorageService(BlobContainerClient blobContainerClient, QueueClient queueClient)
    : IImageStorageService
{
    public async Task<string> UploadImageAsync(string filename, byte[] data, string contentType,
        CancellationToken cancellationToken)
    {
        var blobClient = blobContainerClient.GetBlobClient(filename);

        using var stream = new MemoryStream(data);

        var headers = new BlobHttpHeaders
        {
            ContentType = contentType
        };

        await blobClient.UploadAsync(stream, headers, cancellationToken: cancellationToken);
        return blobClient.Uri.ToString();
    }

    public async Task<(byte[] Data, string ContentType)>  DownloadImageAsync(string filename, CancellationToken cancellationToken)
    {
        var blobClient = blobContainerClient.GetBlobClient(filename);
        var exists = await blobClient.ExistsAsync(cancellationToken);
        if (!exists.Value)
        {
            return ([], string.Empty);
        }
        
        var download = await blobClient.DownloadContentAsync(cancellationToken);

        var content = download.Value.Content.ToArray();
        var contentType = download.Value.Details.ContentType;

        return (content, contentType);
    }

    public async Task DeleteImageAsync(string filename, CancellationToken cancellationToken)
    {
        var blobClient = blobContainerClient.GetBlobClient(filename);
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public async Task EnqueueImageProcessingAsync(string message, CancellationToken cancellationToken)
    {
        await queueClient.SendMessageAsync(message, cancellationToken: cancellationToken);
    }
}
