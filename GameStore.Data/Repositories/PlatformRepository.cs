using GameStore.BLL.Interfaces.Repositories;
using GameStore.Data.Data;
using GameStore.Domain.Entities.CoreEntities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Data.Repositories;

public class PlatformRepository(GameStoreContext context) : IPlatformRepository
{
    public async Task<IEnumerable<Platform>> GetAllPlatformsAsync(CancellationToken cancellationToken)
    {
        return await context.Platforms
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Platform> GetPlatformByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Platforms
            .Include(p => p.Games)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task AddPlatformAsync(Platform platform, CancellationToken cancellationToken)
    {
        await context.Platforms.AddAsync(platform, cancellationToken);
    }

    public void UpdatePlatformAsync(Platform platform)
    {
        context.Platforms.Update(platform);
    }

    public async Task DeletePlatformAsync(Guid id, CancellationToken cancellationToken)
    {
        var platform = await GetPlatformByIdAsync(id, cancellationToken);
        context.Platforms.Remove(platform);
    }

    public async Task<bool> PlatformExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Platforms.AsNoTracking().AnyAsync(p => p.Id == id, cancellationToken);
    }
}
