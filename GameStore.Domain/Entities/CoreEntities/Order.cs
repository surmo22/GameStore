using System.ComponentModel.DataAnnotations.Schema;
using GameStore.Common.Constants;
using GameStore.Domain.Entities.Relations;

namespace GameStore.Domain.Entities.CoreEntities;

public class Order
{
    public Guid Id { get; set; }

    public DateTime? Date { get; set; }

    public Guid CustomerId { get; set; }

    public OrderStatuses Status { get; set; }

    public ICollection<OrderGame> Items { get; set; }

    [NotMapped]
    public double? TotalSum => Items.Sum(item =>
    {
        var discount = Math.Clamp(item.Discount ?? 0, 0, 100);
        var discountFactor = 1 - discount / 100.0;
        return Math.Max(0, item.Price * discountFactor * item.Quantity);
    });
}
