using GameStore.ImageCompressor.Interfaces;
using SixLabors.ImageSharp;

namespace GameStore.ImageCompressor.Services;

public class ImageCompressionService(
    IImageStorageService imageStorageService,
    IImageEncoderFactory imageEncoderFactory,
    IImageResizer imageResizer)
    : IImageCompressionService
{
    public async Task CompressAndUploadImageAsync(string filename, CancellationToken cancellationToken)
    {
        var originalBytes = await imageStorageService.DownloadImageAsync(filename, cancellationToken);  

        using var inputStream = new MemoryStream(originalBytes);
        var format = await Image.DetectFormatAsync(inputStream, cancellationToken);
        inputStream.Position = 0;

        using var image = await Image.LoadAsync(inputStream, cancellationToken);

        imageResizer.Resize(image);

        using var outputStream = new MemoryStream();

        var encoder = imageEncoderFactory.CreateEncoder(format.Name);

        await image.SaveAsync(outputStream, encoder, cancellationToken);
        outputStream.Position = 0;

        await imageStorageService.UploadImageAsync(filename, outputStream, cancellationToken);
    }
}