using System.Text.Json.Serialization;

namespace GameStore.BLL.DTOs.Payment;

public class PaymentRequest
{
    [JsonPropertyName("method")]
    public string Method { get; set; }

    [JsonPropertyName("model")]
    public VisaPaymentData? Visa { get; set; }
}