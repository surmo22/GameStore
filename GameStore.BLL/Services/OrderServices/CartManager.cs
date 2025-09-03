using GameStore.BLL.DTOs.Games;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Interfaces.OrderServices;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.Entities.Relations;

namespace GameStore.BLL.Services.OrderServices;

public class CartManager(IGuidProvider guidProvider) : ICartManager
{
    public void AddGameToOrder(Order order, GameDto game)
    {
        var orderItem = order.Items.FirstOrDefault(oi => oi.ProductId == game.Id);
        if (orderItem is null)
        {
            order.Items.Add(new OrderGame()
            {
                Id = guidProvider.NewGuid(),
                OrderId = order.Id,
                ProductId = game.Id,
                Quantity = 1,
                Price = game.Price,
                Discount = game.Discount,
            });
        }
        else
        {
            orderItem.Quantity++;
        }
    }

    public void RemoveGameFromOrder(Order order, GameDto game)
    {
        var orderItem = order.Items.FirstOrDefault(oi => oi.ProductId == game.Id);
        if (orderItem is null)
        {
            return;
        }

        if (orderItem.Quantity > 1)
        {
            orderItem.Quantity--;
        }
        else
        {
            order.Items.Remove(orderItem);
        }
    }
}