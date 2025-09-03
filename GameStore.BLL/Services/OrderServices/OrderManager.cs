using GameStore.BLL.Interfaces;
using GameStore.BLL.Interfaces.OrderServices;
using GameStore.BLL.Interfaces.Repositories;
using GameStore.Common.Constants;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Services.OrderServices;

public class OrderManager(IGuidProvider guidProvider, IUnitOfWork unitOfWork) : IOrderManager
{
    public async Task<Order> GetOrCreateOpenOrderAsync(Guid customerId, CancellationToken cancellationToken)
    {
        var order = await unitOfWork.Orders.GetCustomersOpenOrderAsync(customerId, cancellationToken);
        if (order is not null)
        {
            return order;
        }

        order = new Order
            { Id = guidProvider.NewGuid(), CustomerId = customerId, Status = OrderStatuses.Open, Items = [], };
        await unitOfWork.Orders.AddAsync(order);
        await unitOfWork.CompleteAsync(cancellationToken);

        return order;
    }

    public async Task DeleteOrderIfEmptyAsync(Order order, CancellationToken cancellationToken)
    {
        if (order.Items.Count == 0)
        {
            await unitOfWork.Orders.DeleteAsync(order.Id, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
        }
    }
}