using GameStore.ImageCompressor.Interfaces;
using GameStore.ImageCompressor.Utils;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;

namespace GameStore.ImageCompressor.Services;

public class ImageEncoderFactory(IOptions<ImageCompressorOptions> options) : IImageEncoderFactory
{
    private readonly ImageCompressorOptions _options = options.Value;

    public IImageEncoder CreateEncoder(string formatName)
    {
        return formatName.ToUpperInvariant() switch
        {
            "JPEG" or "JPG" => new JpegEncoder { Quality = _options.ImageQuality },
            "PNG" => new PngEncoder(),
            "GIF" => new GifEncoder(),
            _ => throw new NotSupportedException($"Unsupported format: {formatName}")
        };
    }
}