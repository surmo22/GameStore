using GameStore.ImageCompressor.Interfaces;
using GameStore.ImageCompressor.Utils;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace GameStore.ImageCompressor.Services;

public class ImageResizer(IOptions<ImageCompressorOptions> options) : IImageResizer
{
    private readonly ImageCompressorOptions _options = options.Value;

    public void Resize(Image image)
    {
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(_options.ImageResizeWidth, _options.ImageResizeHeight),
            Mode = ResizeMode.Max
        }));
    }
}