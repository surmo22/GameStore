using System.Text.Json.Serialization;

namespace GameStore.BLL.DTOs.Payment;

public class BoxPaymentDto
{
    [JsonPropertyName("userId")]
    public Guid UserId { get; set; }

    [JsonPropertyName("orderId")]
    public Guid OrderId { get; set; }

    [JsonPropertyName("paymentDate")]
    public DateTime PaymentDate { get; set; } = DateTime.Now;

    [JsonPropertyName("sum")]
    public double Sum { get; set; }
}
