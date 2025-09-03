using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GameStore.BLL.DTOs.Publisher;

public class PublisherCreateDto
{
    [Required(ErrorMessage = "Company name is required")]
    [StringLength(50, ErrorMessage = "Company name is too long")]
    [JsonPropertyName("companyName")]
    public string CompanyName { get; set; }

    [Url(ErrorMessage = "Home page is not a valid URL")]
    [Required(ErrorMessage = "HomePage is required")]
    [JsonPropertyName("homePage")]
    public string HomePage { get; set; }

    [StringLength(400, ErrorMessage = "Description is too long")]
    [Required(ErrorMessage = "Description is required")]
    [JsonPropertyName("description")]
    public string Description { get; set; }
}
