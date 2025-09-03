using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Interfaces.Repositories;

public interface IPlatformRepository
{
    Task<IEnumerable<Platform>> GetAllPlatformsAsync(CancellationToken cancellationToken);

    Task<Platform> GetPlatformByIdAsync(Guid id, CancellationToken cancellationToken);

    Task AddPlatformAsync(Platform platform, CancellationToken cancellationToken);

    void UpdatePlatformAsync(Platform platform);

    Task DeletePlatformAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> PlatformExistsAsync(Guid id, CancellationToken cancellationToken);
}
