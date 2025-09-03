using GameStore.BLL.Interfaces;
using GameStore.Data.Data;
using GameStore.Data.Interfaces;
using GameStore.Domain.MongoEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GameStore.Data.Utils;

public class EntityChangeInterceptor(IMongoLogger logger, IDateTimeProvider timeProvider) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, 
        InterceptionResult<int> result, 
        CancellationToken cancellationToken = default)
    {
        var dbContext = (GameStoreContext)eventData.Context;

        var changes = dbContext.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Select(e => new Log()
            {
                Timestamp = timeProvider.UtcNow,
                Action = e.State.ToString(),
                EntityType = e.Entity.GetType().Name,
                OldVersion = e.State is EntityState.Modified or EntityState.Deleted ? e.OriginalValues.ToObject() : null,
                NewVersion = e.State is EntityState.Added or EntityState.Modified ? e.CurrentValues.ToObject() : null
            }).ToList();

        if (changes.Count != 0)
        {
            await logger.LogChangesAsync(changes, cancellationToken);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
