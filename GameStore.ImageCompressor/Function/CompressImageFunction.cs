using GameStore.ImageCompressor.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace GameStore.ImageCompressor.Function;

public class CompressImageFunction(IImageCompressionService imageCompressionService)
{
    [Function("CompressImageFunction")]
    public async Task Run(
        [QueueTrigger("imageproccessing", Connection = "AzureWebJobsStorage")] string? filename,
        FunctionContext context)
    {
        var logger = context.GetLogger("CompressImageFunction");
        logger.LogInformation($"Processing image: {filename}");
        ThrowIfFilenameIsNullOrEmpty(filename, logger);

        try
        {
            await imageCompressionService.CompressAndUploadImageAsync(filename!, context.CancellationToken);
            logger.LogInformation("Image compressed and re-uploaded: {filename}", filename);
        }
        catch (Exception ex)
        {
            logger.LogError("Error processing image {filename}: {ex}", filename, ex);
            throw;
        }
    }

    private static void ThrowIfFilenameIsNullOrEmpty(string? filename, ILogger logger)
    {
        if (!string.IsNullOrEmpty(filename))
        {
            return;
        }

        logger.LogError("Got empty filename. {Filename}", filename);
        throw new ArgumentNullException(nameof(filename));
    }
}