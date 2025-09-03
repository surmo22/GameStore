using SixLabors.ImageSharp;

namespace GameStore.ImageCompressor.Interfaces;

public interface IImageResizer
{
    void Resize(Image image);
}