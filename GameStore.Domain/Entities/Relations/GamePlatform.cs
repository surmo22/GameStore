using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.Domain.Entities.Relations;

public class GamePlatform
{
    public Guid GameId { get; set; }

    public Game Game { get; set; }

    public Guid PlatformId { get; set; }

    public Platform Platform { get; set; }
}