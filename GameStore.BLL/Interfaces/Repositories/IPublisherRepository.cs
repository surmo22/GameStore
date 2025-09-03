using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Interfaces.Repositories;

public interface IPublisherRepository
{
    Task AddPublisherAsync(Publisher publisher, CancellationToken cancellationToken);

    Task<IEnumerable<Publisher>> GetAllPublishersAsync(CancellationToken cancellationToken);

    Task<Publisher?> GetPublisherByIdAsync(Guid id, CancellationToken cancellationToken);

    Task UpdatePublisher(Publisher publisher, CancellationToken cancellationToken);

    Task DeletePublisherAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> PublisherExistsAsync(Guid id, CancellationToken cancellationToken);

    Task<Publisher?> GetPublisherByCompanyNameAsync(string companyName, CancellationToken cancellationToken);
}