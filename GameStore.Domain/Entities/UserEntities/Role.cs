using Microsoft.AspNetCore.Identity;

namespace GameStore.Domain.Entities.UserEntities;

public class Role : IdentityRole<Guid>
{
    public bool IsSystemRole { get; set; }
    
    public virtual ICollection<IdentityRoleClaim<Guid>> Claims { get; set; } = new List<IdentityRoleClaim<Guid>>();
}