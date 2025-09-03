using GameStore.BLL.Interfaces.GameServices.ImageServices;

namespace GameStore.BLL.Services.GameServices.ImageServices;

public class ImageMimeMapper : IImageMimeMapper
{
    public string GetExtension(string mimeType)
    {
        return mimeType.ToLowerInvariant() switch
        {
            "image/png" => ".png",
            "image/jpeg" => ".jpg",
            "image/gif" => ".gif",
            _ => throw new NotSupportedException($"Unsupported image format: {mimeType}")
        };
    }
}