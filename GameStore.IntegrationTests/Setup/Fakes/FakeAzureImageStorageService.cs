using GameStore.BLL.Interfaces.ExternalServices;

namespace GameStore.IntegrationTests.Setup.Fakes;

public class FakeAzureImageStorageService : IImageStorageService
{
    public Task<string> UploadImageAsync(string filename, byte[] data, string contentType, CancellationToken cancellationToken)
    {
        return Task.FromResult("test.png");
    }

    public Task<(byte[] Data, string ContentType)> DownloadImageAsync(string filename, CancellationToken cancellationToken)
    {
        return Task.FromResult((Array.Empty<byte>(), ""));
    }

    public Task DeleteImageAsync(string? filename, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task EnqueueImageProcessingAsync(string message, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}