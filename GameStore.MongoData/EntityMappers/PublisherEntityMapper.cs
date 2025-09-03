using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.MongoData.EntityMappers;

public static class PublisherEntityMapper
{
    public static IEnumerable<Publisher> MapPublishersByKey(IEnumerable<Publisher> sqlPublishers, IEnumerable<Publisher> mongoPublishers)
    {
        var publishers = new List<Publisher>(sqlPublishers);

        foreach (var mongoPublisher in mongoPublishers)
        {
            var exists = publishers.Any(p => p.CompanyName == mongoPublisher.CompanyName);
            if (!exists)
            {
                publishers.Add(mongoPublisher);
            }
        }

        return publishers.Where(p => !p.IsDeleted);
    }
}