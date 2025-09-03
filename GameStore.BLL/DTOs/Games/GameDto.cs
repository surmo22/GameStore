using System.ComponentModel.DataAnnotations;

namespace GameStore.BLL.DTOs.Games;

public class GameDto
{
    [Required(ErrorMessage = "ID is required")]
    public Guid Id { get; set; }

    [StringLength(50, ErrorMessage = "Key must be at most 100 characters long")]
    [MinLength(3, ErrorMessage = "Key minimum length is 3 characters")]
    public string Key { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(50, ErrorMessage = "Name must be at most 100 characters long")]
    [MinLength(3, ErrorMessage = "Name minimum length is 3 characters")]
    public string Name { get; set; }

    [StringLength(400, ErrorMessage = "Description should be more than 400 symbols")]
    public string? Description { get; set; }

    [Range(0, 10000, ErrorMessage = "Price should be more than 0 and less than 10000")]
    public double Price { get; set; }

    [Range(0, 200000, ErrorMessage = "Unit in stock should be more than 0 and less than 10000")]
    public int UnitInStock { get; set; }

    [Range(minimum: 0, maximum: 100, ErrorMessage = "Discount should be more than 0 and less than 100")]
    public int Discount { get; set; }
}