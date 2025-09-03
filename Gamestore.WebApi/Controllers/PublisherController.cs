using GameStore.BLL.DTOs.Games;
using GameStore.BLL.DTOs.Publisher;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.Domain.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.WebApi.Controllers;

[Route("publishers")]
[ApiController]
public class PublisherController(IPublisherService publisherService) : ControllerBase
{
    /// <summary>
    /// Creates publisher in the database.
    /// </summary>
    /// <param name="publisherCreateDto">Publisher data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created publisher.</returns>
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [Authorize(Policy = nameof(UserPermissionTypes.ManageEntities))]
    public async Task<ActionResult<PublisherDto>> CreatePublisher(PublisherCreateRequest publisherCreateDto, CancellationToken cancellationToken)
    {
        var publisherDto = await publisherService.CreatePublisherAsync(publisherCreateDto.Publisher, cancellationToken);
        return CreatedAtAction(nameof(GetPublisherByCompanyName), new { companyName = publisherDto.CompanyName }, publisherDto);
    }

    /// <summary>
    /// Deletes publisher from the database.
    /// </summary>
    /// <param name="id">Id to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [Authorize(Policy = nameof(UserPermissionTypes.ManageEntities))]
    public async Task<IActionResult> DeletePublisher(Guid id, CancellationToken cancellationToken)
    {
        await publisherService.DeletePublisherAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Returns all publishers from the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>All publishers.</returns>
    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<ActionResult<IEnumerable<PublisherDto>>> GetAllPublishers(CancellationToken cancellationToken)
    {
        var publishers = await publisherService.GetAllPublishersAsync(cancellationToken);
        return Ok(publishers);
    }

    /// <summary>
    /// Gets publisher by name.
    /// </summary>
    /// <param name="companyName">name to search for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Publisher.</returns>
    [HttpGet("{companyName}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<PublisherDto>> GetPublisherByCompanyName(string companyName, CancellationToken cancellationToken)
    {
        var publisher = await publisherService.GetPublisherByCompanyNameAsync(companyName, cancellationToken);
        return Ok(publisher);
    }

    /// <summary>
    /// Updates publisher in the database.
    /// </summary>
    /// <param name="publisherDto">Publisher data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpPut]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [Authorize(Policy = nameof(UserPermissionTypes.ManageEntities))]
    public async Task<IActionResult> UpdatePublisher(PublisherRequest publisherDto, CancellationToken cancellationToken)
    {
        await publisherService.UpdatePublisherAsync(publisherDto.Publisher, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Returns games by publisher id.
    /// </summary>
    /// <param name="companyName">company name to look for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of games.</returns>
    [HttpGet("{companyName}/games")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IEnumerable<GameDto>>> GetGamesByPublisherName(string companyName, CancellationToken cancellationToken)
    {
        var games = await publisherService.GetGamesByPublisherCompanyNameAsync(companyName, cancellationToken);
        return Ok(games);
    }
}
