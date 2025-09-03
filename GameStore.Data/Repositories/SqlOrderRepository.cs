using GameStore.BLL.Interfaces.Repositories;
using GameStore.Common.Constants;
using GameStore.Data.Data;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.Entities.Relations;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Data.Repositories;

public class SqlOrderRepository(GameStoreContext context) : IOrderRepository
{
    public async Task AddAsync(Order order)
    {
        await context.Orders.AddAsync(order);
    }

    public async Task<IList<Order>> GetAllOrdersByDate(DateTime startDate, DateTime endDate,
        CancellationToken cancellationToken)
    {
        return await context.Orders.Where(o => o.Date > startDate && o.Date < endDate)
            .OrderBy(o => o.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(Guid customerId, CancellationToken token)
    {
        return await context.Orders
            .Include(o => o.Items)
            .Where(o => o.CustomerId == customerId)
            .ToListAsync(token);
    }

    public async Task UpdateAsync(Order order, CancellationToken token)
    {
        var foundOrder = await context.Orders
            .Include(o => o.Items)
            .FirstAsync(o => o.Id == order.Id, token);
        context.Entry(foundOrder).CurrentValues.SetValues(order);
        foundOrder.Items = order.Items.Select(item => new OrderGame()
        {
            Id = item.Id,
            ProductId = item.ProductId,
            Quantity = item.Quantity,
            Price = item.Price,
            OrderId = item.OrderId,
            Discount = item.Discount
        }).ToList();
    }

    public async Task DeleteAsync(Guid id, CancellationToken token)
    {
        var order = await context.Orders.FirstAsync(o => o.Id == id, token);
        context.Orders.Remove(order);
    }

    public async Task<bool> OrderExistsAsync(Guid id, CancellationToken token)
    {
        return await context.Orders.AnyAsync(o => o.Id == id, token);
    }

    public async Task<Order?> GetOrderByIdAsync(Guid id, CancellationToken token)
    {
        return await context.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .ThenInclude(o => o.Product)
            .FirstOrDefaultAsync(o => o.Id == id, token);
    }
    
    public async Task<Order?> GetCustomersOpenOrderAsync(Guid customerId, CancellationToken token)
    {
        return await context.Orders.AsNoTracking()
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.CustomerId == customerId && o.Status == OrderStatuses.Open, token);
    }

    public async Task<IList<Order>> GetPaidAndCancelledOrdersAsync(CancellationToken cancellationToken)
    {
        return await context.Orders
            .Include(o => o.Items)
            .Where(o => o.Status == OrderStatuses.Paid || o.Status == OrderStatuses.Cancelled)
            .ToListAsync(cancellationToken);
    }

    public async Task<Order> GetOrderByItemIdAsync(Guid itemId, CancellationToken token)
    {
        return await context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Items.Any(i => i.Id == itemId), token);
    }
}