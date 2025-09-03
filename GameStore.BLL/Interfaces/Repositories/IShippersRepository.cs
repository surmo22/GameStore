using GameStore.Domain.MongoEntities;

namespace GameStore.BLL.Interfaces.Repositories;

public interface IShippersRepository
{
    Task<IEnumerable<MongoShipper>> GetAllAsync(CancellationToken cancellationToken);
}