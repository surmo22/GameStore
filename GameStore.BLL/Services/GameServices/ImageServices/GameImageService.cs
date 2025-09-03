using GameStore.BLL.Interfaces.ExternalServices;
using GameStore.BLL.Interfaces.GameServices.ImageServices;

namespace GameStore.BLL.Services.GameServices.ImageServices;

public class GameImageService(
    IImageDataParser imageDataParser,
    IImageStorageService storageService) : IGameImageService
{
    public async Task<string> UploadImageAsync(string gameKey, string imageData, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(imageData))
        {
            return string.Empty;
        }

        var (imageBytes, mimeType, extension) = imageDataParser.ParseBase64Image(imageData);

        var filename = $"{gameKey}{extension}";

        await storageService.UploadImageAsync(filename, imageBytes, mimeType, cancellationToken);

        await storageService.EnqueueImageProcessingAsync(filename, cancellationToken);

        return filename;
    }

    public async Task DeleteImageAsync(string? filename, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(filename))
        {
            return;
        }
        
        await storageService.DeleteImageAsync(filename, cancellationToken);
    }

    public async Task<(byte[] Data, string ContentType)> GetImage(string? filename, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(filename))
        {
            return ([], string.Empty);
        }
        
        return await storageService.DownloadImageAsync(filename, cancellationToken);
    }
}