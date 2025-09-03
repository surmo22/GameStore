using GameStore.BLL.DTOs.Games;
using GameStore.BLL.DTOs.Platforms;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.Domain.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.WebApi.Controllers;

[Route("platforms")]
[ApiController]
public class PlatformsController(IPlatformService platformService) : ControllerBase
{
    /// <summary>
    /// Creates platform in the database.
    /// </summary>
    /// <param name="platformCreateDto">Platform information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created platform.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Policy = nameof(UserPermissionTypes.ManageEntities))]
    public async Task<ActionResult<PlatformDto>> CreatePlatform([FromBody] PlatformCreateRequest platformCreateDto, CancellationToken cancellationToken)
    {
        var result = await platformService.CreatePlatformAsync(platformCreateDto.Platform, cancellationToken);
        return CreatedAtAction(nameof(GetPlatformById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Deletes platform from the database.
    /// </summary>
    /// <param name="id">Platform id to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Policy = nameof(UserPermissionTypes.ManageEntities))]
    public async Task<IActionResult> DeletePlatform(Guid id, CancellationToken cancellationToken)
    {
        await platformService.DeletePlatformAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Returns all platforms from the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of the platforms.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PlatformDto>>> GetAllPlatforms(CancellationToken cancellationToken)
    {
        var platforms = await platformService.GetAllPlatformsAsync(cancellationToken);
        return Ok(platforms);
    }

    /// <summary>
    /// Gets platform by its id from the database.
    /// </summary>
    /// <param name="id">id to look for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Found platform.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PlatformDto>> GetPlatformById(Guid id, CancellationToken cancellationToken)
    {
        var platform = await platformService.GetPlatformByIdAsync(id, cancellationToken);
        return platform is null ? NotFound($"Platform with ID {id} not found.") : Ok(platform);
    }

    /// <summary>
    /// Updates platform in the database.
    /// </summary>
    /// <param name="platformDto">Updated platform information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Policy = nameof(UserPermissionTypes.ManageEntities))]
    public async Task<IActionResult> UpdatePlatform([FromBody] PlatformRequest platformDto, CancellationToken cancellationToken)
    {
        await platformService.UpdatePlatformAsync(platformDto.Platform, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Gets games by platform id.
    /// </summary>
    /// <param name="id">Platform id to look for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of games related to the platform.</returns>
    [HttpGet("{id:guid}/games")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<GameDto>>> GetGamesByPlatformId(Guid id, CancellationToken cancellationToken)
    {
        var games = await platformService.GetGamesByPlatformIdAsync(id, cancellationToken);
        return Ok(games);
    }
}
