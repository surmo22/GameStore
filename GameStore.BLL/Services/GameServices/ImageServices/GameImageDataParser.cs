using GameStore.BLL.Interfaces.GameServices.ImageServices;

namespace GameStore.BLL.Services.GameServices.ImageServices;

public class GameImageDataParser(IImageMimeMapper mimeMapper) : IImageDataParser
{
    public (byte[] ImageBytes, string MimeType, string Extension) ParseBase64Image(string base64Image)
    {
        if (string.IsNullOrWhiteSpace(base64Image))
        {
            throw new InvalidOperationException("Image data is empty.");
        }

        var dataParts = base64Image.Split(',');
        if (dataParts.Length != 2 || !dataParts[0].StartsWith("data:image", StringComparison.InvariantCulture))
        {
            throw new InvalidOperationException("Invalid image data format.");
        }

        var mimeTypeHeader = dataParts[0];
        var base64Data = dataParts[1];

        var mimeType = mimeTypeHeader.Split(':')[1].Split(';')[0];

        var extension = mimeMapper.GetExtension(mimeType);

        var imageBytes = Convert.FromBase64String(base64Data);
        return (imageBytes, mimeType, extension);
    }
}