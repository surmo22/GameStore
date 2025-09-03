using GameStore.BLL.DTOs.Games;
using GameStore.BLL.DTOs.Genres;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.Domain.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.WebApi.Controllers;

/// <summary>
/// Represents a controller to handle operations related to genres in the system.
/// </summary>
[Route("genres")]
[ApiController]
public class GenresController(IGenreService genreService) : ControllerBase
{
    /// <summary>
    /// Creates genre in the database.
    /// </summary>
    /// <param name="genreCreateDto">Genre data.</param>
    /// <param name="cancellationToken">CancellationToken.</param>
    /// <returns>Created genre.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Policy = nameof(UserPermissionTypes.ManageEntities))]
    public async Task<ActionResult<GenreDto>> CreateGenre([FromBody] GenreCreateRequest genreCreateDto, CancellationToken cancellationToken)
    {
        var result = await genreService.CreateGenreAsync(genreCreateDto.Genre, cancellationToken);
        return CreatedAtAction(nameof(GetGenreById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Deletes genre from the database.
    /// </summary>
    /// <param name="id">id of the genre to delete.</param>
    /// <param name="cancellationToken">CancellationToken.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = nameof(UserPermissionTypes.ManageEntities))]
    public async Task<IActionResult> DeleteGenre(Guid id, CancellationToken cancellationToken)
    {
        await genreService.DeleteGenreAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Get all genres from the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of all genres.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GenreDto>>> GetAllGenres(CancellationToken cancellationToken)
    {
        var genres = await genreService.GetAllGenresAsync(cancellationToken);
        return Ok(genres);
    }

    /// <summary>
    /// Gets genre by id from the database.
    /// </summary>
    /// <param name="id">id to look for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Found genre.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GenreDto>> GetGenreById(Guid id, CancellationToken cancellationToken)
    {
        var genre = await genreService.GetGenreByIdAsync(id, cancellationToken);
        return genre is null ?
            NotFound($"Genre with ID {id} not found.") :
            Ok(genre);
    }

    /// <summary>
    /// Updates genre in the database.
    /// </summary>
    /// <param name="genreDto">Updated genre information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Policy = nameof(UserPermissionTypes.ManageEntities))]
    public async Task<IActionResult> UpdateGenre([FromBody] GenreRequest genreDto, CancellationToken cancellationToken)
    {
        await genreService.UpdateGenreAsync(genreDto.Genre, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Returns child genres of the genre.
    /// </summary>
    /// <param name="id">id to search for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of nested genres.</returns>
    [HttpGet("{id:guid}/genres")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<GenreDto>>> GetNestedGenres(Guid id, CancellationToken cancellationToken)
    {
        var genres = await genreService.GetNestedGenresAsync(id, cancellationToken);
        return Ok(genres);
    }

    /// <summary>
    /// Get games by genre id.
    /// </summary>
    /// <param name="id">Genre id to look for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of games related to genre.</returns>
    [HttpGet("{id:guid}/games")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<GameDto>>> GetGamesByGenreId(Guid id, CancellationToken cancellationToken)
    {
        var games = await genreService.GetGamesByGenreIdAsync(id, cancellationToken);
        return Ok(games);
    }
}