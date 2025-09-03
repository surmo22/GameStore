using System.ComponentModel.DataAnnotations;

namespace GameStore.BLL.DTOs.Publisher;

public class PublisherDto
{
    [Required(ErrorMessage = "ID is required")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Company Name is required")]
    [StringLength(50, ErrorMessage = "Company Name is too long")]
    public string CompanyName { get; set; }

    [Url(ErrorMessage = "Home Page is not a valid URL")]
    public string? HomePage { get; set; }

    [StringLength(400, ErrorMessage = "Description is too long")]
    public string? Description { get; set; }
}