namespace GameStore.Domain.Entities.CoreEntities;

public class Publisher
{
    public Guid Id { get; set; }

    public string CompanyName { get; set; }

    public string? HomePage { get; set; }

    public string? Description { get; set; }

    public bool IsDeleted { get; set; }

    public ICollection<Game> Games { get; set; }
}