namespace GameStore.BLL.Interfaces.GameServices.ImageServices;

/// <summary>
/// Provides methods for handling game-related image operations such as uploading,
/// retrieving, and deleting images.
/// </summary>
public interface IGameImageService
{
    /// <summary>
    /// Uploads an image associated with a specific game, processes it, and returns the filename.
    /// </summary>
    /// <param name="gameKey">The unique identifier of the game to which the image belongs.</param>
    /// <param name="imageData">The Base64 encoded image data to be uploaded.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the filename of the uploaded image.</returns>
    Task<string> UploadImageAsync(string gameKey, string imageData, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an image file with the specified filename.
    /// </summary>
    /// <param name="filename">The name of the file to be deleted.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteImageAsync(string? filename, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves an image file's data and content type based on its filename.
    /// </summary>
    /// <param name="filename">The name of the file to retrieve.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A tuple containing the image data as a byte array and the content type as a string.</returns>
    Task<(byte[] Data, string ContentType)> GetImage(string? filename, CancellationToken cancellationToken);
}