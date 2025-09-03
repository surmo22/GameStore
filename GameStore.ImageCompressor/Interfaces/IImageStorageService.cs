namespace GameStore.ImageCompressor.Interfaces;

public interface IImageStorageService
{
    Task<byte[]> DownloadImageAsync(string filename, CancellationToken cancellationToken);
    Task UploadImageAsync(string filename, Stream imageStream, CancellationToken cancellationToken);
}