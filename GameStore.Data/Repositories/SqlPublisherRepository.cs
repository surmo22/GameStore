using GameStore.BLL.Interfaces.Repositories;
using GameStore.Data.Data;
using GameStore.Domain.Entities.CoreEntities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Data.Repositories;

public class SqlPublisherRepository(GameStoreContext context) : IPublisherRepository
{
    public async Task AddPublisherAsync(Publisher publisher, CancellationToken cancellationToken)
    {
        await context.Publishers.AddAsync(publisher, cancellationToken);
    }

    public async Task<IEnumerable<Publisher>> GetAllPublishersAsync(CancellationToken cancellationToken)
    {
        return await context.Publishers
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Publisher?> GetPublisherByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Publishers
            .Include(p => p.Games)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task UpdatePublisher(Publisher publisher, CancellationToken cancellationToken)
    {
        context.Publishers.Update(publisher);
        return Task.CompletedTask;
    }

    public async Task DeletePublisherAsync(Guid id, CancellationToken cancellationToken)
    {
        var publisher = await context.Publishers.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        publisher.IsDeleted = true;
        context.Publishers.Update(publisher);
    }

    public async Task<bool> PublisherExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Publishers.AsNoTracking().AnyAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Publisher?> GetPublisherByCompanyNameAsync(string companyName, CancellationToken cancellationToken)
    {
        return await context.Publishers.AsNoTracking()
            .Include(p => p.Games)
            .FirstOrDefaultAsync(p => p.CompanyName == companyName, cancellationToken);
    }
}