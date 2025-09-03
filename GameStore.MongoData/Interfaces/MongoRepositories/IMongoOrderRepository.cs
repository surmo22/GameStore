using GameStore.Domain.MongoEntities;

namespace GameStore.MongoData.Interfaces.MongoRepositories;

public interface IMongoOrderRepository
{
    Task<IEnumerable<MongoOrder>> GetAllOrdersAsync(CancellationToken cancellationToken);
    Task<IEnumerable<MongoOrder>> GetAllOrdersAsync(DateTime startDate, DateTime endDate,
        CancellationToken cancellationToken);
    Task<MongoOrder> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken);
    Task<IEnumerable<MongoOrderDetails>> GetOrderDetailsByIdAsync(Guid orderId, CancellationToken cancellationToken);
}