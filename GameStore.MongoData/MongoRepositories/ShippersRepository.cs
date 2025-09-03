using GameStore.BLL.Interfaces.Repositories;
using GameStore.Data.Data;
using GameStore.Domain.MongoEntities;
using MongoDB.Driver;

namespace GameStore.MongoData.MongoRepositories;

public class ShippersRepository(NorthwindMongoContext context) : IShippersRepository
{
    public async Task<IEnumerable<MongoShipper>> GetAllAsync(CancellationToken cancellationToken)
    {
        var result = await context.Shippers.Find(_ => true).ToListAsync(cancellationToken);
        return result;
    }
}