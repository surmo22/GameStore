using System.Text.Json.Serialization;

namespace GameStore.BLL.DTOs.Comment;

public class AddCommentDto
{
    [JsonPropertyName("body")]
    public string Body { get; set; }
}