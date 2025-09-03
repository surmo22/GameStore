using System.ComponentModel.DataAnnotations;

namespace GameStore.BLL.DTOs.Genres;

public class GenreCreateDto
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }

    public string? ParentGenreId { get; set; }
}
