using GameStore.BLL.Interfaces.Repositories;
using GameStore.Data.Data;
using GameStore.Domain.Entities.CoreEntities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Data.Repositories;

public class PaymentMethodsRepository(GameStoreContext context) : IPaymentMethodsRepository
{
    public async Task<IEnumerable<PaymentMethod>> GetAllPaymentMethodsAsync(CancellationToken cancellationToken)
    {
        return await context.PaymentMethods
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}