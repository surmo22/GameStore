namespace GameStore.BLL.Interfaces.GameServices.ImageServices;

/// <summary>
/// Provides functionality to parse base64 encoded image data into its components, including the binary image data, MIME type, and file extension.
/// </summary>
public interface IImageDataParser
{
    /// Parses a Base64-encoded image string and extracts its binary data, MIME type, and file extension.
    /// <param name="base64Image">A Base64 string containing image data in the format "data:[MIME type];base64,[data]".</param>
    /// <returns>A tuple containing:
    /// - ImageBytes: The decoded binary data of the image.
    /// - MimeType: The MIME type of the image (e.g., "image/png").
    /// - Extension: The file extension associated with the MIME type (e.g., ".png").</returns>
    /// <exception cref="InvalidOperationException">Thrown when the input image data is null, empty, or in an invalid format.</exception>
    (byte[] ImageBytes, string MimeType, string Extension) ParseBase64Image(string base64Image);
}