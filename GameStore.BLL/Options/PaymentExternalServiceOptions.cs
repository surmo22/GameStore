namespace GameStore.BLL.Options;

public class PaymentExternalServiceOptions
{
    public string BaseUrl { get; set; }
    
    public int MaxAttempts { get; init; }

    public int DelayInSeconds { get; init; }
}