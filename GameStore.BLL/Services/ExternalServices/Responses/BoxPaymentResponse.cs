using System.Text.Json.Serialization;

namespace GameStore.BLL.Services.ExternalServices.Responses;

public class BoxPaymentResponse
{
    [JsonPropertyName("accountNumber")]
    public Guid UserId { get; set; }

    [JsonPropertyName("invoiceNumber")]
    public string OrderId { get; set; }

    [JsonPropertyName("paymentMethod")]
    public int PaymentMethod { get; set; }

    [JsonPropertyName("accountId")]
    public Guid AccountId { get; set; }

    [JsonPropertyName("amount")]
    public double Amount { get; set; }
}
