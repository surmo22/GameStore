using System.ComponentModel.DataAnnotations;

namespace GameStore.BLL.DTOs.Platforms;

public class PlatformDto
{
    [Required(ErrorMessage = "ID is required")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Type is required")]
    public string Type { get; set; }
}
