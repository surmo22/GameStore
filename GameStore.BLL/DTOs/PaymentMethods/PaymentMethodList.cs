using System.Text.Json.Serialization;

namespace GameStore.BLL.DTOs.PaymentMethods;

public class PaymentMethodList
{
    [JsonPropertyName("paymentMethods")]
    public List<PaymentMethodDto> PaymentMethods { get; set; }
}