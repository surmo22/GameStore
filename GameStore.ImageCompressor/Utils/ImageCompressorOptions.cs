namespace GameStore.ImageCompressor.Utils;

public class ImageCompressorOptions
{
    public string QueueName { get; set; } = null!;
    
    public string BlobContainerName { get; set; } = null!;
    
    public int ImageResizeWidth { get; set; } = 600;
    
    public int ImageResizeHeight { get; set; } = 800;
    
    public int ImageQuality { get; set; } = 75;
}