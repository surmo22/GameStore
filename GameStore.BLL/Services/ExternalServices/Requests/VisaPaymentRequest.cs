using System.Text.Json.Serialization;

namespace GameStore.BLL.Services.ExternalServices.Requests;

public class VisaPaymentRequest
{
    [JsonPropertyName("transactionAmount")]
    public double TransactionAmount { get; set; }

    [JsonPropertyName("cardHolderName")]
    public string CardHolderName { get; set; }

    [JsonPropertyName("cardNumber")]
    public string CardNumber { get; set; }

    [JsonPropertyName("expirationMonth")]
    public int ExpirationMonth { get; set; }

    [JsonPropertyName("expirationYear")]
    public int ExpirationYear { get; set; }

    [JsonPropertyName("cvv")]
    public int Cvv { get; set; }
}
