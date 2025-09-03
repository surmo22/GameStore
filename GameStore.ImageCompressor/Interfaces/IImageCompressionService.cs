namespace GameStore.ImageCompressor.Interfaces;

public interface IImageCompressionService
{
    Task CompressAndUploadImageAsync(string filename, CancellationToken cancellationToken);
}