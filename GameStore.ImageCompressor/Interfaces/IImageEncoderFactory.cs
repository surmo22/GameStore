using SixLabors.ImageSharp.Formats;

namespace GameStore.ImageCompressor.Interfaces;

public interface IImageEncoderFactory
{
    IImageEncoder CreateEncoder(string formatName);
}