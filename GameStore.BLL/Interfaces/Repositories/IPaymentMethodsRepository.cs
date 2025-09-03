using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Interfaces.Repositories;

public interface IPaymentMethodsRepository
{
    public Task<IEnumerable<PaymentMethod>> GetAllPaymentMethodsAsync(CancellationToken cancellationToken);
}