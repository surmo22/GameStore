namespace GameStore.Domain.Entities.CoreEntities;

public class Game
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Key { get; set; }
    
    public string? ImageUrl { get; set; }

    public string? Description { get; set; }

    public double Price { get; set; }

    public int UnitInStock { get; set; }

    public int Discount { get; set; }
    
    public DateTime CreationDate { get; set; }

    public int ViewCount { get; set; }

    public bool IsDeleted { get; set; }
    
    public string? QuantityPerUnit { get; set; }
    
    public int? UnitsOnOrder { get; set; }
    
    public int? ReorderLevel { get; set; }
    
    public bool? Discontinued { get; set; }

    public Guid PublisherId { get; set; }

    public Publisher Publisher { get; set; }

    public ICollection<Comment> Comments { get; set; } = [];

    public ICollection<Genre> Genres { get; set; } = [];

    public ICollection<Platform> Platforms { get; set; } = [];
}
