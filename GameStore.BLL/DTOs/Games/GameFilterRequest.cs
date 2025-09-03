using System.ComponentModel.DataAnnotations;
using GameStore.Common.Constants;

namespace GameStore.BLL.DTOs.Games;

public class GameFilterRequest
{
    // Name filter
    [MinLength(3, ErrorMessage = "Minimum length for game name is 3 characters")]
    public string? Name { get; set; }

    // Genres
    public List<Guid> Genres { get; set; } = [];

    // Platforms
    public List<Guid> Platforms { get; set; } = [];
    
    // Publisher
    public Guid? Publisher { get; set; }
    
    // Publishing Date
    public PublishingDateFilter? DatePublishing { get; set; } 
    
    // Price
    public double? MinPrice { get; set; } = 0;
    
    public double? MaxPrice { get; set; } = double.MaxValue;
    
    // Sorting
    public SortingOptions? Sort { get; set; }
    
    // Pagination
    public int Page { get; set; } = 1;

    public string? PageCount { get; set; }
}