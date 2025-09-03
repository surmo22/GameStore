using System.ComponentModel.DataAnnotations;

namespace GameStore.BLL.DTOs.Platforms;

public class PlatformCreateDto
{
    [Required(ErrorMessage = "Type is required")]
    public string Type { get; set; }
}
