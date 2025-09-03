namespace GameStore.Domain.Entities.CoreEntities;

public class Platform
{
    public Guid Id { get; set; }

    public string Type { get; set; }

    public ICollection<Game> Games { get; set; }
}
