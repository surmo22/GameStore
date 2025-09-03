using GameStore.BLL.Interfaces.Repositories;
using GameStore.Data.Data;
using GameStore.Domain.Entities.Relations;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Data.Repositories;

public class UserBanRepository(GameStoreContext context) : IUserBanRepository
{
    public async Task AddAsync(UserBan userBan, CancellationToken token)
    {
        await context.UserBans.AddAsync(userBan, token);
    }

    public async Task<bool> IsUserBannedAsync(Guid id, DateTime currentDateTime)
    {
        return await context.UserBans
            .AsNoTracking()
            .AnyAsync(ub => ub.UserId == id && ub.BannedUntil > currentDateTime);
    }
}