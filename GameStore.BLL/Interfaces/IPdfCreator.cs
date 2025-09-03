using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Interfaces;

public interface IPdfCreator
{
    public byte[] GenerateInvoicePdf(Order order, int daysValid);
}