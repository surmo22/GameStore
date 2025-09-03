using AutoMapper;
using GameStore.BLL.Interfaces.Repositories;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.MongoData.EntityMappers;
using GameStore.MongoData.Interfaces.MongoRepositories;

namespace GameStore.MongoData.SharedRepositories;

public class PublisherRepository(IPublisherRepository sqlPublisherRepository, IMongoPublisherRepository mongoPublisherRepository, IMapper mapper) : IPublisherRepository
{
    public async Task AddPublisherAsync(Publisher publisher, CancellationToken cancellationToken)
    {
        await sqlPublisherRepository.AddPublisherAsync(publisher, cancellationToken);
    }

    public async Task<IEnumerable<Publisher>> GetAllPublishersAsync(CancellationToken cancellationToken)
    {
        var sqlPublishersTask = sqlPublisherRepository.GetAllPublishersAsync(cancellationToken);
        var mongoPublishersTask = mongoPublisherRepository.GetAllPublishersAsync(cancellationToken);
        
        await Task.WhenAll(sqlPublishersTask, mongoPublishersTask);
        
        var sqlPublishers = await sqlPublishersTask;
        var mongoPublishers = await mongoPublishersTask;
        var mappedMongoPublishers = mapper.Map<IEnumerable<Publisher>>(mongoPublishers);
        
        return PublisherEntityMapper.MapPublishersByKey(sqlPublishers, mappedMongoPublishers);
    }

    public async Task<Publisher?> GetPublisherByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var publisher = await sqlPublisherRepository.GetPublisherByIdAsync(id, cancellationToken);

        if (publisher is not null)
        {
            return publisher;
        }
        
        var mongoPublisher = await mongoPublisherRepository.GetPublisherByIdAsync(id, cancellationToken);
        var mappedMongoPublisher = mapper.Map<Publisher>(mongoPublisher);
        
        return mappedMongoPublisher;
    }

    public async Task UpdatePublisher(Publisher publisher, CancellationToken cancellationToken)
    {
        if (await sqlPublisherRepository.PublisherExistsAsync(publisher.Id, cancellationToken))
        {
            await sqlPublisherRepository.UpdatePublisher(publisher, cancellationToken);
            return;
        }
        
        await sqlPublisherRepository.AddPublisherAsync(publisher, cancellationToken);
    }

    public async Task DeletePublisherAsync(Guid id, CancellationToken cancellationToken)
    {
        if (await sqlPublisherRepository.PublisherExistsAsync(id, cancellationToken))
        {
            await sqlPublisherRepository.DeletePublisherAsync(id, cancellationToken);
            return;
        }
        var mongoPublisher = await mongoPublisherRepository.GetPublisherByIdAsync(id, cancellationToken);
        var mappedPublisher = mapper.Map<Publisher>(mongoPublisher);
        mappedPublisher.IsDeleted = true;
        
        await sqlPublisherRepository.AddPublisherAsync(mappedPublisher, cancellationToken);
    }

    public async Task<bool> PublisherExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await sqlPublisherRepository.PublisherExistsAsync(id, cancellationToken) 
               || await mongoPublisherRepository.PublisherExistsAsync(id, cancellationToken);
    }

    public async Task<Publisher?> GetPublisherByCompanyNameAsync(string companyName, CancellationToken cancellationToken)
    {
        var sqlPublisher = await sqlPublisherRepository.GetPublisherByCompanyNameAsync(companyName, cancellationToken);
        if (sqlPublisher is not null)
        {
            return sqlPublisher;
        }
        
        var mongoPublisher = await mongoPublisherRepository.GetPublisherByCompanyNameAsync(companyName, cancellationToken);
        var mappedMongoPublisher = mapper.Map<Publisher>(mongoPublisher);
        return mappedMongoPublisher;
    }
}