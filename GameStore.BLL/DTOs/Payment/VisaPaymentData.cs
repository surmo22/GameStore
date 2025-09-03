using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GameStore.BLL.DTOs.Payment;

public class VisaPaymentData
{
    [JsonPropertyName("holder")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Invalid holder name")]
    public string Holder { get; set; }

    [JsonPropertyName("cardNumber")]
    [StringLength(16, MinimumLength = 16, ErrorMessage = "Invalid card number")]
    [RegularExpression(@"^4\d{15}$", ErrorMessage = "Visa card must start with 4")]
    public string CardNumber { get; set; }

    [JsonPropertyName("monthExpire")]
    [Range(1, 12, ErrorMessage = "Invalid month")]
    public int MonthExpire { get; set; }

    [JsonPropertyName("yearExpire")]
    [Range(2024, 2100, ErrorMessage = "Invalid year")]
    public int YearExpire { get; set; }

    [JsonPropertyName("cvv2")]
    [Range(100, 999, ErrorMessage = "Invalid cvv")]
    public int Cvv2 { get; set; }
}