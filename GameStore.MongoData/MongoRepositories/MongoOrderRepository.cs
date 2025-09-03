using GameStore.Common.Utils;
using GameStore.Data.Data;
using GameStore.Domain.MongoEntities;
using GameStore.MongoData.Interfaces.MongoRepositories;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace GameStore.MongoData.MongoRepositories;

public class MongoOrderRepository(NorthwindMongoContext context) : IMongoOrderRepository
{
    public async Task<IEnumerable<MongoOrder>> GetAllOrdersAsync(CancellationToken cancellationToken)
    {
        var orders = await context.Orders.AsQueryable().ToListAsync(cancellationToken);
        return orders;
    }
    
    public async Task<IEnumerable<MongoOrder>> GetAllOrdersAsync(DateTime startDate, DateTime endDate,
        CancellationToken cancellationToken)
    {
        var orders = await context.Orders.AsQueryable()
            .Where(o => o.OrderDate > startDate && o.OrderDate < endDate)
            .ToListAsync(cancellationToken);
        
        return orders;
    }

    public async Task<MongoOrder> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var intId = IntToGuidConverter.Convert(orderId);
        var order = await context.Orders
            .AsQueryable()
            .Where(order => order.OrderId == intId)
            .FirstOrDefaultAsync(cancellationToken);
        return order;
    }

    public async Task<IEnumerable<MongoOrderDetails>> GetOrderDetailsByIdAsync(Guid orderId,
        CancellationToken cancellationToken)
    {
        var intId = IntToGuidConverter.Convert(orderId);
        var orderDetails = await context.OrderDetails
            .AsQueryable()
            .Where(order => order.OrderId == intId)
            .ToListAsync(cancellationToken);
        return orderDetails;
    }
}