namespace GameStore.AzureStorage.Options;

public class AzureStorageOptions
{
    public string ConnectionString { get; set; }
    
    public string BlobContainerName { get; set; }
    
    public string QueueName { get; set; }
}