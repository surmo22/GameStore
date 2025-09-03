using GameStore.BLL.DTOs.Comment;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.BLL.Interfaces.Repositories;
using GameStore.Common.Constants;
using GameStore.Domain.Entities.Relations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace GameStore.BLL.Services.EntityServices;

public class UserBanService(
    IUnitOfWork             unitOfWork,
    IDateTimeProvider       timeProvider,
    IUserService            userService,
    IMemoryCache            memoryCache,
    ILogger<UserBanService> logger) : IUserBanService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
    
    public async Task<bool> IsUserBannedAsync(Guid id, CancellationToken cancellationToken)
    {
        if (memoryCache.TryGetValue(id, out bool cachedIsBanned))
        {
            return cachedIsBanned;
        }
        
        var now = timeProvider.UtcNow;
        var isBanned = await unitOfWork.UserBans.IsUserBannedAsync(id, now);
        
        memoryCache.Set(id, isBanned, CacheDuration);

        return isBanned;
    }

    public async Task BanUserAsync(string userName, BanUserRequest request, CancellationToken cancellationToken)
    {
        var currentDateTime = timeProvider.UtcNow;
        var bannedUntil = currentDateTime.Add(request.Duration.ConvertBanDurationToTimeSpan());
        var user = await userService.GetUserIdByUserNameAsync(userName);
        var userBan = new UserBan
        {
            UserId = user,
            BannedUntil = bannedUntil,
        };

        await unitOfWork.UserBans.AddAsync(userBan, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        logger.LogInformation($"User {userName} was banned until {bannedUntil}");
    }
}