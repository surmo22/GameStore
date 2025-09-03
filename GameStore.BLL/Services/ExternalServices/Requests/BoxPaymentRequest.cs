using System.Text.Json.Serialization;

namespace GameStore.BLL.Services.ExternalServices.Requests;

public class BoxPaymentRequest
{
    [JsonPropertyName("transactionAmount")]
    public double TransactionAmount { get; set; }

    [JsonPropertyName("accountNumber")]
    public Guid AccountId { get; set; }

    [JsonPropertyName("invoiceNumber")]
    public Guid OrderId { get; set; }
}