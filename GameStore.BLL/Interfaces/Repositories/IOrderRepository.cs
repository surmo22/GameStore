using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Interfaces.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order);

    Task<Order?> GetOrderByIdAsync(Guid id, CancellationToken token);

    Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(Guid customerId, CancellationToken token);

    Task UpdateAsync(Order order, CancellationToken token);

    Task DeleteAsync(Guid id, CancellationToken token);

    Task<bool> OrderExistsAsync(Guid id, CancellationToken token);

    Task<Order?> GetCustomersOpenOrderAsync(Guid customerId, CancellationToken token);

    Task<IList<Order>> GetPaidAndCancelledOrdersAsync(CancellationToken cancellationToken);
    Task<Order> GetOrderByItemIdAsync(Guid itemId, CancellationToken token);

    Task<IList<Order>> GetAllOrdersByDate(DateTime startDate, DateTime endDate,
        CancellationToken cancellationToken);
}