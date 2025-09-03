using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using GameStore.BLL.Attributes;

namespace GameStore.BLL.DTOs.Games;

public class GameUpdateRequestDto : IGameRequest
{
    public GameDto Game { get; set; }

    public List<Guid> Genres { get; set; }

    public List<Guid> Platforms { get; set; }

    [Required(ErrorMessage = "Publisher is required")]
    [NotEmptyGuid]
    [JsonPropertyName("Publisher")]
    public Guid PublisherId { get; set; }
    
    public string Image { get; set; }
}
