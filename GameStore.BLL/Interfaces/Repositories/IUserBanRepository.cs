using GameStore.Domain.Entities.Relations;

namespace GameStore.BLL.Interfaces.Repositories;

public interface IUserBanRepository
{
    Task AddAsync(UserBan userBan, CancellationToken token);

    Task<bool> IsUserBannedAsync(Guid id, DateTime currentDateTime);
}