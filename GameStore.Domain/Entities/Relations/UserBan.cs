using GameStore.Domain.Entities.UserEntities;

namespace GameStore.Domain.Entities.Relations;

public class UserBan
{
    public Guid UserId { get; set; }
    
    public User User { get; set; }

    public DateTime BannedUntil { get; set; }
}
