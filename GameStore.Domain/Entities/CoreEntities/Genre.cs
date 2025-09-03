namespace GameStore.Domain.Entities.CoreEntities;

public class Genre
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public bool IsDeleted { get; set; }

    public Guid? ParentGenreId { get; set; }

    public Genre ParentGenre { get; set; }

    public ICollection<Genre> SubGenres { get; set; }

    public ICollection<Game> Games { get; set; }
}
