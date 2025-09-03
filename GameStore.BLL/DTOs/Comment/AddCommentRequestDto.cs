using System.Text.Json.Serialization;

namespace GameStore.BLL.DTOs.Comment;

public class AddCommentRequestDto
{
    [JsonPropertyName("comment")]
    public AddCommentDto AddComment { get; set; }

    [JsonPropertyName("parentId")]
    public string? ParentId { get; set; }

    [JsonPropertyName("action")]
    public string Action { get; set; }
}