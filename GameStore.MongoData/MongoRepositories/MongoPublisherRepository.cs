using GameStore.Common.Utils;
using GameStore.Data.Data;
using GameStore.Domain.MongoEntities;
using GameStore.MongoData.Interfaces.MongoRepositories;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace GameStore.MongoData.MongoRepositories;

public class MongoPublisherRepository(NorthwindMongoContext context) : IMongoPublisherRepository
{
    public async Task<IEnumerable<MongoPublisher>> GetAllPublishersAsync(CancellationToken cancellationToken)
    {
        var mongoPublishers = await context.Publishers.AsQueryable().ToListAsync(cancellationToken);

        return mongoPublishers;
    }

    public async Task<MongoPublisher?> GetPublisherByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var intId = IntToGuidConverter.Convert(id);

        var mongoPublisher = await context.Publishers.AsQueryable().Where(p => p.SupplierId == intId).FirstOrDefaultAsync(cancellationToken);

        return mongoPublisher;
    }

    public async Task<bool> PublisherExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        var intId = IntToGuidConverter.Convert(id);
        var filter = Builders<MongoPublisher>.Filter.Eq("SupplierID", intId);
        var result = await context.Publishers.Find(filter).AnyAsync(cancellationToken);

        return result;
    }

    public async Task<MongoPublisher> GetPublisherByCompanyNameAsync(string companyName, CancellationToken cancellationToken)
    {
        var filter = Builders<MongoPublisher>.Filter.Eq("CompanyName", companyName);

        var mongoResult = await context.Publishers.Find(filter).FirstOrDefaultAsync(cancellationToken);

        return mongoResult;
    }
}