using AutoMapper;
using GameStore.BLL.Interfaces.Repositories;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.Entities.Relations;
using GameStore.MongoData.Interfaces;
using GameStore.MongoData.Interfaces.MongoRepositories;

namespace GameStore.MongoData.SharedRepositories;

public class OrderRepository(IOrderRepository sqlRepository,
    IMongoOrderRepository mongoOrderRepository,
    IGameRepository gameRepository,
    IDatabaseMigrator migrator,
    IMapper mapper) : IOrderRepository
{
    public async Task AddAsync(Order order)
    {
        await sqlRepository.AddAsync(order);
    }

    public async Task<Order?> GetOrderByIdAsync(Guid id, CancellationToken token)
    {
        var order = await sqlRepository.GetOrderByIdAsync(id, token);
        if (order is not null)
        {
            return order;
        }
        
        var mongoOrder = await mongoOrderRepository.GetOrderByIdAsync(id, token);
        var mongoOrderDetails = await mongoOrderRepository.GetOrderDetailsByIdAsync(id, token);
        
        var mappedOrder = mapper.Map<Order>(mongoOrder);
        var mappedOrderDetails = mapper.Map<IEnumerable<OrderGame>>(mongoOrderDetails);

        mappedOrder.Items = mappedOrderDetails.ToList();
        return mappedOrder;
    }

    public async Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(Guid customerId, CancellationToken token)
    {
        var sqlOrdersTask = sqlRepository.GetOrdersByCustomerIdAsync(customerId, token);
        var mongoOrdersTask = mongoOrderRepository.GetAllOrdersAsync(token);
        
        await Task.WhenAll(sqlOrdersTask, mongoOrdersTask);
        
        var sqlOrders = await sqlOrdersTask;
        var mongoOrders = await mongoOrdersTask;
        var mappedOrders = mapper.Map<IEnumerable<Order>>(mongoOrders);
        
        return sqlOrders.Concat(mappedOrders);
    }

    public async Task UpdateAsync(Order order, CancellationToken token)
    {
        foreach (var orderGame in order.Items)
        {
            var game = await gameRepository.GetGameByIdAsync(orderGame.ProductId, token);
            if (game is not null)
            {
                await migrator.MigrateGameAsync(game, game, token);
            }
        }
        
        await sqlRepository.UpdateAsync(order, token);
    }

    public async Task DeleteAsync(Guid id, CancellationToken token)
    {
        await sqlRepository.DeleteAsync(id, token);
    }

    public async Task<bool> OrderExistsAsync(Guid id, CancellationToken token)
    {
        return await sqlRepository.OrderExistsAsync(id, token);
    }

    public async Task<Order?> GetCustomersOpenOrderAsync(Guid customerId, CancellationToken token)
    {
        return await sqlRepository.GetCustomersOpenOrderAsync(customerId, token);
    }

    public async Task<IList<Order>> GetPaidAndCancelledOrdersAsync(CancellationToken cancellationToken)
    {
        var sqlOrdersTask = sqlRepository.GetPaidAndCancelledOrdersAsync(cancellationToken);
        var mongoOrdersTask = mongoOrderRepository.GetAllOrdersAsync(cancellationToken);
        
        await Task.WhenAll(sqlOrdersTask, mongoOrdersTask);
        
        var sqlOrders = await sqlOrdersTask;
        var mongoOrders = await mongoOrdersTask;
        var mappedOrders = mapper.Map<IEnumerable<Order>>(mongoOrders);
        
        return sqlOrders.Concat(mappedOrders).ToList();
    }

    public async Task<Order> GetOrderByItemIdAsync(Guid itemId, CancellationToken token)
    {
        var sqlOrder = await sqlRepository.GetOrderByItemIdAsync(itemId, token);
        return sqlOrder ?? throw new KeyNotFoundException("Order not found");
    }

    public async Task<IList<Order>> GetAllOrdersByDate(DateTime startDate, DateTime endDate,
        CancellationToken cancellationToken)
    {
        var sqlOrdersTask = sqlRepository.GetAllOrdersByDate(startDate, endDate, cancellationToken);
        var mongoOrdersTask = mongoOrderRepository.GetAllOrdersAsync(startDate, endDate, cancellationToken);

        await Task.WhenAll(sqlOrdersTask, mongoOrdersTask);
        
        var sqlOrders = await sqlOrdersTask;
        var mongoOrders = await mongoOrdersTask;
        var mappedOrders = mapper.Map<IEnumerable<Order>>(mongoOrders);
        
        return sqlOrders.Concat(mappedOrders).ToList();
    }
}