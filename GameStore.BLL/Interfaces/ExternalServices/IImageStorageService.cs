namespace GameStore.BLL.Interfaces.ExternalServices;

/// <summary>
/// Provides functionality for image storage and processing within an external service.
/// </summary>
public interface IImageStorageService
{
    /// <summary>
    /// Uploads an image to a storage service and returns the URL of the uploaded image.
    /// </summary>
    /// <param name="filename">The desired name of the image file to be stored.</param>
    /// <param name="data">The byte array containing the image data to be uploaded.</param>
    /// <param name="contentType">The MIME type of the image being uploaded.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the URL of the uploaded image.</returns>
    Task<string> UploadImageAsync(string filename, Byte[] data, string contentType, CancellationToken cancellationToken);

    /// Asynchronously downloads an image file from the storage by its filename.
    /// <param name="filename">The name of the file to be downloaded.</param>
    /// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
    /// <returns>A tuple containing the image data as a byte array and its content type.</returns>
    Task<(byte[] Data, string ContentType)> DownloadImageAsync(string filename, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an image with the specified filename from the storage asynchronously.
    /// </summary>
    /// <param name="filename">The name of the image file to delete.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    Task DeleteImageAsync(string filename, CancellationToken cancellationToken);

    /// <summary>
    /// Enqueues a message for asynchronous image processing.
    /// </summary>
    /// <param name="message">The message to enqueue, the image file name.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task EnqueueImageProcessingAsync(string message, CancellationToken cancellationToken);
}