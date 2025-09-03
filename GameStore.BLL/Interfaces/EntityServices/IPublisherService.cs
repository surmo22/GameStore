using GameStore.BLL.DTOs.Games;
using GameStore.BLL.DTOs.Publisher;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.Interfaces.EntityServices;

/// <summary>
/// Defines the contract for managing publishers in the system.
/// </summary>
public interface IPublisherService
{
    /// <summary>
    /// Asynchronously creates a new publisher using the provided data and returns the newly created publisher details.
    /// </summary>
    /// <param name="publisherCreateDto">The data transfer object containing information required to create a publisher.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation if needed.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the details of the newly created publisher.</returns>
    Task<PublisherDto> CreatePublisherAsync(PublisherCreateDto publisherCreateDto, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a publisher with the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the publisher to delete.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    Task DeletePublisherAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a collection of all publishers as asynchronous operation.
    /// </summary>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see cref="PublisherDto"/>.</returns>
    Task<IEnumerable<PublisherDto>> GetAllPublishersAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a publisher by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the publisher to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the publisher data transfer object.</returns>
    Task<PublisherDto> GetPublisherByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the details of an existing publisher.
    /// </summary>
    /// <param name="publisherDto">The publisher data transfer object containing updated publisher information.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdatePublisherAsync(PublisherDto publisherDto, CancellationToken cancellationToken);

    /// Retrieves a collection of games associated with a specific publisher company name.
    /// <param name="companyName">The name of the publisher's company to filter games by.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a collection of GameDto representing the games of the specified publisher.</returns>
    Task<IEnumerable<GameDto>> GetGamesByPublisherCompanyNameAsync(string companyName, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the publisher details based on the provided company name.
    /// </summary>
    /// <param name="companyName">The company name of the publisher to be retrieved.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the <see cref="PublisherDto"/> corresponding to the specified company name.</returns>
    Task<PublisherDto> GetPublisherByCompanyNameAsync(string companyName, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a publisher entity by its unique identifier without mapping it to a DTO.
    /// </summary>
    /// <param name="id">The unique identifier of the publisher to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the publisher entity with the specified identifier.</returns>
    Task<Publisher> GetPublisherByIdNotMappedAsync(Guid id, CancellationToken cancellationToken);
}