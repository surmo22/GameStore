using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.Domain.Entities.Relations;

public class OrderGame
{
    public Guid Id { get; set; }
    
    public Guid OrderId { get; set; }

    public Order Order { get; set; }

    public Guid ProductId { get; set; }

    public Game Product { get; set; }

    public double Price { get; set; }

    public int Quantity { get; set; }

    public int? Discount { get; set; }
}