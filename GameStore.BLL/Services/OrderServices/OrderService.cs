using AutoMapper;
using GameStore.BLL.DTOs.Orders;
using GameStore.BLL.DTOs.Payment;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.BLL.Interfaces.NotificationServices;
using GameStore.BLL.Interfaces.OrderServices;
using GameStore.BLL.Interfaces.PaymentServices;
using GameStore.BLL.Interfaces.Repositories;
using GameStore.Common.Constants;
using GameStore.Common.Exceptions;
using GameStore.Domain.Entities.CoreEntities;
using Microsoft.Extensions.Logging;

namespace GameStore.BLL.Services.OrderServices;

public class OrderService(
    IOrderServices orderServices,
    IGameService gameService,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IDateTimeProvider dateTimeProvider,
    INotificationService notificationService,
    ILogger<OrderService> logger) : IOrderService
{
    public async Task AddGameToCartAsync(Guid userId, string gameKey, CancellationToken cancellationToken)
    {
        logger.LogInformation("Adding game with id {GameKey} to the customer's cart with {CustomerId}", gameKey, userId);
        var game = await gameService.GetGameByKeyAsync(gameKey, cancellationToken) ?? throw new KeyNotFoundException($"No Game was found with key {gameKey}");
        var order = await orderServices.OrderManager.GetOrCreateOpenOrderAsync(userId, cancellationToken);

        orderServices.CartManager.AddGameToOrder(order, game);

        await unitOfWork.Orders.UpdateAsync(order, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        logger.LogInformation("Game with id {GameKey} was added to the customer's cart with {CustomerId}", gameKey, userId);
    }

    public async Task RemoveGameFromCartAsync(Guid userId, string gameKey, CancellationToken cancellationToken)
    {
        logger.LogInformation("Removing game with id {GameKey} from the customer's cart with {CustomerId}", gameKey, userId);
        var game = await gameService.GetGameByKeyAsync(gameKey, cancellationToken) ?? throw new KeyNotFoundException($"No Game was found with key {gameKey}");
        var order = await unitOfWork.Orders.GetCustomersOpenOrderAsync(userId, cancellationToken) ?? throw new KeyNotFoundException($"No open order was found for customer");

        orderServices.CartManager.RemoveGameFromOrder(order, game);

        await unitOfWork.Orders.UpdateAsync(order, cancellationToken);
        await orderServices.OrderManager.DeleteOrderIfEmptyAsync(order, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        logger.LogInformation("Game with id {GameKey} was removed from the customer's cart with {CustomerId}", gameKey, userId);
    }

    public async Task<IList<OrderDto>> GetPaidAndCancelledOrdersAsync(CancellationToken cancellationToken)
    {
        var orders = await unitOfWork.Orders.GetPaidAndCancelledOrdersAsync(cancellationToken);

        return orders.Select(mapper.Map<OrderDto>).ToList();
    }

    public async Task<OrderDto> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await unitOfWork.Orders.GetOrderByIdAsync(id, cancellationToken);
        return mapper.Map<OrderDto>(order);
    }

    public async Task<IList<OrderGameDto>> GetOrderDetailsByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await unitOfWork.Orders.GetOrderByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException($"No Order was found with id {id}");
        return order.Items.Select(mapper.Map<OrderGameDto>).ToList();
    }

    public async Task<IList<OrderGameDto>> GetUserOpenOrderAsync(Guid user, CancellationToken cancellationToken)
    {
        var order = await unitOfWork.Orders.GetCustomersOpenOrderAsync(user, cancellationToken);
        return order is null
            ? []
            : order.Items.Select(mapper.Map<OrderGameDto>).ToList();
    }

    public async Task<IPaymentResult> PayForOrderAsync(Guid user, PaymentRequest paymentRequest, CancellationToken cancellationToken)
    {
        var order = await unitOfWork.Orders.GetCustomersOpenOrderAsync(user, cancellationToken) ??
                    throw new KeyNotFoundException($"No open order was found for customer");
        try
        {
            VerifyEnoughUnitsInStock(order);
            
            var result = await orderServices.PaymentService.PayForOrderAsync(order, paymentRequest, cancellationToken);
            order.Status = OrderStatuses.Paid;
            order.Date = dateTimeProvider.UtcNow;
            foreach (var orderItem in order.Items)
            {
                await gameService.UpdateGameStockAsync(orderItem.ProductId, -orderItem.Quantity, cancellationToken);
            }
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Payment for order with id {OrderId} failed", order.Id);
            order.Status = OrderStatuses.Cancelled;
            throw new PaymentFailedException("Payment failed", ex);
        }
        finally
        {
            await unitOfWork.Orders.UpdateAsync(order, CancellationToken.None);
            await unitOfWork.CompleteAsync(CancellationToken.None);
            await notificationService.SendNotificationAboutOrderStatusToRelatedUsers(order, CancellationToken.None);
        }
    }

    public async Task ShipOrderAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await unitOfWork.Orders.GetOrderByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"No Order was found with id {id}");
        if (order.Status == OrderStatuses.Shipped)
        {
            return;
        }
        
        if (order.Status != OrderStatuses.Paid)
        {
            throw new InvalidOperationException("Order is not paid");
        }
        
        order.Status = OrderStatuses.Shipped;
        
        await unitOfWork.Orders.UpdateAsync(order, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        
        await notificationService.SendNotificationAboutOrderStatusToRelatedUsers(order, CancellationToken.None);
        logger.LogInformation("Order with id {OrderId} was shipped", order.Id);
    }

    public async Task DeleteOrderItem(Guid id, CancellationToken cancellationToken)
    {
        var order = await unitOfWork.Orders.GetOrderByItemIdAsync(id, cancellationToken);
        var item = order.Items.First(x => x.Id == id);
        order.Items.Remove(item);
        if (order.Items.Count == 0)
        {
            order.Status = OrderStatuses.Cancelled;
        }
        
        await unitOfWork.Orders.UpdateAsync(order, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
    }

    public async Task UpdateOrderDetailsQuantity(Guid id, int count, CancellationToken cancellationToken)
    {
        var order = await unitOfWork.Orders.GetOrderByItemIdAsync(id, cancellationToken);
        
        var orderItem = order.Items.First(x => x.Id == id);
        orderItem.Quantity = count;
        await unitOfWork.Orders.UpdateAsync(order, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
    }

    public async Task AddGameToOrder(Guid id, string key, CancellationToken cancellationToken)
    {
        var order = await unitOfWork.Orders.GetOrderByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"No Order was found with id {id}");
        var game = await gameService.GetGameByKeyAsync(key, cancellationToken) 
                   ?? throw new KeyNotFoundException($"No Game was found with key {key}");
        
        orderServices.CartManager.AddGameToOrder(order, game);
        await unitOfWork.Orders.UpdateAsync(order, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
    }

    public async Task<IList<OrderDto>> GetOrderHistory(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        var orders = await unitOfWork.Orders.GetAllOrdersByDate(startDate, endDate, cancellationToken);
        return orders.Select(mapper.Map<OrderDto>).ToList();
    }

    private static void VerifyEnoughUnitsInStock(Order order)
    {
        if (order.Items.Any(orderItem => orderItem.Product.UnitInStock - orderItem.Quantity < 0))
        {
            throw new InvalidOperationException("We don't have enough stock to complete the order");
        }
    }
}