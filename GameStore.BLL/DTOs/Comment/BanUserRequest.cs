using System.Text.Json.Serialization;
using GameStore.Common.Constants;

namespace GameStore.BLL.DTOs.Comment;

public class BanUserRequest
{
    [JsonPropertyName("user")]
    public string UserName { get; set; }

    [JsonPropertyName("duration")]
    public BanDuration Duration { get; set; }
}