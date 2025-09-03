using GameStore.Domain.MongoEntities;

namespace GameStore.MongoData.Interfaces.MongoRepositories;

public interface IMongoPublisherRepository
{
    Task<IEnumerable<MongoPublisher>> GetAllPublishersAsync(CancellationToken cancellationToken);
    Task<MongoPublisher?> GetPublisherByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> PublisherExistsAsync(Guid id, CancellationToken cancellationToken);
    Task<MongoPublisher> GetPublisherByCompanyNameAsync(string companyName, CancellationToken cancellationToken);
}