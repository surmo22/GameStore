using System.ComponentModel.DataAnnotations;

namespace GameStore.BLL.DTOs.Genres;

public class GenreDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(30, ErrorMessage = "Name should be at most 30 characters long")]
    public string Name { get; set; }

    public string? ParentGenreId { get; set; }
}
