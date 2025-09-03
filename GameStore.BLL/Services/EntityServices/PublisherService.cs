using AutoMapper;
using GameStore.BLL.DTOs.Games;
using GameStore.BLL.DTOs.Publisher;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.BLL.Interfaces.Repositories;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Services.EntityServices;

public class PublisherService(IGuidProvider guidProvider, IUnitOfWork unitOfWork, IMapper mapper) : IPublisherService
{
    public async Task<PublisherDto> CreatePublisherAsync(PublisherCreateDto publisherCreateDto, CancellationToken cancellationToken)
    {
        var publisher = mapper.Map<Publisher>(publisherCreateDto);
        publisher.Id = guidProvider.NewGuid();
        await unitOfWork.Publishers.AddPublisherAsync(publisher, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return mapper.Map<PublisherDto>(publisher);
    }

    public async Task DeletePublisherAsync(Guid id, CancellationToken cancellationToken)
    {
        await EnsurePublisherExistsAsync(id, cancellationToken);

        await unitOfWork.Publishers.DeletePublisherAsync(id, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
    }

    public async Task<IEnumerable<PublisherDto>> GetAllPublishersAsync(CancellationToken cancellationToken)
    {
        var publishers = await unitOfWork.Publishers.GetAllPublishersAsync(cancellationToken);
        return mapper.Map<IEnumerable<PublisherDto>>(publishers);
    }

    public async Task<PublisherDto> GetPublisherByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var publisher = await unitOfWork.Publishers.GetPublisherByIdAsync(id, cancellationToken) 
                        ?? throw new KeyNotFoundException($"No publisher was found with id: {id}");
        return mapper.Map<PublisherDto>(publisher);
    }
    
    public async Task<Publisher> GetPublisherByIdNotMappedAsync(Guid id, CancellationToken cancellationToken)
    {
        var publisher = await unitOfWork.Publishers.GetPublisherByIdAsync(id, cancellationToken) 
                        ?? throw new KeyNotFoundException($"No publisher was found with id: {id}");
        return publisher;
    }

    public async Task UpdatePublisherAsync(PublisherDto publisherDto, CancellationToken cancellationToken)
    {
        var publisher = mapper.Map<Publisher>(publisherDto);
        await EnsurePublisherExistsAsync(publisher.Id, cancellationToken);

        await unitOfWork.Publishers.UpdatePublisher(publisher, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
    }

    public async Task<IEnumerable<GameDto>> GetGamesByPublisherCompanyNameAsync(string companyName, CancellationToken cancellationToken)
    {
        var publisher = await unitOfWork.Publishers.GetPublisherByCompanyNameAsync(companyName, cancellationToken) ?? throw new KeyNotFoundException($"No Publisher was found with Id {companyName}");
        var games = publisher.Games;
        return mapper.Map<IEnumerable<GameDto>>(games);
    }

    public async Task<PublisherDto> GetPublisherByCompanyNameAsync(string companyName, CancellationToken cancellationToken)
    {
        var publisher = await unitOfWork.Publishers.GetPublisherByCompanyNameAsync(companyName, cancellationToken)
            ?? throw new KeyNotFoundException($"No publisher was found with name: {companyName}");
        return mapper.Map<PublisherDto>(publisher);
    }

    private async Task EnsurePublisherExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!await unitOfWork.Publishers.PublisherExistsAsync(id, cancellationToken))
        {
            throw new KeyNotFoundException($"No Publisher was found with Id {id}");
        }
    }
}