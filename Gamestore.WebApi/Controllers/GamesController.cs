using System.Security.Claims;
using GameStore.BLL.DTOs.Comment;
using GameStore.BLL.DTOs.Games;
using GameStore.BLL.DTOs.Genres;
using GameStore.BLL.DTOs.Platforms;
using GameStore.BLL.DTOs.Publisher;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.BLL.Interfaces.GameServices;
using GameStore.BLL.Interfaces.OrderServices;
using GameStore.Common.Constants;
using GameStore.Domain.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace GameStore.WebApi.Controllers;

/// <summary>
/// Responsible for handling operations related to games within the system such as creating, retrieving, updating, or deleting game resources.
/// </summary>
/// <remarks>
/// The <see cref="GamesController" /> class includes various endpoints for managing games, their associated data (genres, platforms, publishers, etc.), and related actions like purchasing or commenting.
/// </remarks>
[Route("games")]
[ApiController]
public class GamesController(
    IGameService gameService,
    IGameFileService gameFileService,
    IOrderService orderService,
    ICommentService commentService,
    IMemoryCache cache,
    ILogger<GamesController> logger) : ControllerBase
{
    /// <summary>
    /// Creates Game in the database.
    /// </summary>
    /// <param name="gameCreateRequestDto">Request with GameDto and ids of related Genres and Games.</param>
    /// <param name="cancellationToken">Cancellation Token. </param>
    /// <returns>Created game. </returns>
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [Authorize(Policy = nameof(UserPermissionTypes.ManageEntities))]
    public async Task<ActionResult<GameDto>> CreateGame(GameCreateRequestDto gameCreateRequestDto, CancellationToken cancellationToken)
    {
        var gameDto = await gameService.CreateGameAsync(gameCreateRequestDto, cancellationToken);
        return CreatedAtAction(nameof(GetGameByKey), new { gameKey = gameDto.Key }, gameDto);
    }

    /// <summary>
    /// Deletes game from the database.
    /// </summary>
    /// <param name="key">Game key to delete.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken">Cancellation token</see>.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{key}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [Authorize(Policy = nameof(UserPermissionTypes.ManageEntities))]
    public async Task<IActionResult> DeleteGame(string key, CancellationToken cancellationToken)
    {
        await gameService.DeleteGameAsync(key, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Returns all games from the database.
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken">Cancellation token</see>.</param>
    /// <returns>Ok with games.</returns>
    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<ActionResult<IEnumerable<GameDto>>> GetAllGames([FromQuery] GameFilterRequest filterRequest, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(filterRequest.PageCount))
        {
            filterRequest.PageCount = "20";
        }
        
        var games = await gameService.GetAllGamesAsync(filterRequest, cancellationToken);
        return Ok(games);
    }

    /// <summary>
    /// Retrieves a list of all games without applying any filters.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of all games.</returns>
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<GameDto>>> GetAllGamesWithoutFilter(CancellationToken cancellationToken)
    {
        var games = await gameService.GetAllGamesWithoutFilters(cancellationToken);
        return Ok(games);
    }

    /// <summary>
    /// Returns game by key.
    /// </summary>
    /// <param name="gameKey">Key to search for.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken">Cancellation token</see>.</param>
    /// <returns><see cref="NotFoundObjectResult"/> When game isn't found and <see cref="OkObjectResult"/> with game.</returns>
    [HttpGet("{gameKey}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [Authorize(Policy = nameof(UserPermissionTypes.ViewGames))]
    public async Task<ActionResult<GameDto>> GetGameByKey(string gameKey, CancellationToken cancellationToken)
    {
        var game = await gameService.GetGameByKeyAsync(gameKey, cancellationToken);
        return game is null ?
            NotFound($"Game with key {gameKey} not found.") :
            Ok(game);
    }

    /// <summary>
    /// Returns genres by game key.
    /// </summary>
    /// <param name="gameKey">Key of the game.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Genres of the found game.</returns>
    [HttpGet("{gameKey}/genres")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [Authorize(Policy = nameof(UserPermissionTypes.ViewGames))]
    public async Task<ActionResult<IEnumerable<GenreDto>>> GetGenresByGameKey(string gameKey, CancellationToken cancellationToken)
    {
        return Ok(await gameService.GetGenresByGameKeyAsync(gameKey, cancellationToken));
    }

    /// <summary>
    /// Returns publisher by game key.
    /// </summary>
    /// <param name="gameKey">Key of the game.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of publisher.</returns>
    [HttpGet("{gameKey}/publisher")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [Authorize(Policy = nameof(UserPermissionTypes.ViewGames))]
    public async Task<ActionResult<PublisherDto>> GetPublisherByGameKey(string gameKey, CancellationToken cancellationToken)
    {
        return Ok(await gameService.GetPublisherByGameKeyAsync(gameKey, cancellationToken));
    }

    /// <summary>
    /// Returns platforms by game key.
    /// </summary>
    /// <param name="gameKey">Key of the game.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Platforms of the found game.</returns>
    [HttpGet("{gameKey}/platforms")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [Authorize(Policy = nameof(UserPermissionTypes.ViewGames))]
    public async Task<ActionResult<IEnumerable<PlatformDto>>> GetPlatformsByGameKey(string gameKey, CancellationToken cancellationToken)
    {
        return Ok(await gameService.GetPlatformsByGameKeyAsync(gameKey, cancellationToken));
    }

    /// <summary>
    /// Returns game file with serialized game by key.
    /// </summary>
    /// <param name="gameKey">Key to search for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Game file.</returns>
    [HttpGet("{gameKey}/file")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [Authorize(Policy = nameof(UserPermissionTypes.ViewGames))]
    public async Task<IActionResult> DownloadGame(string gameKey, CancellationToken cancellationToken)
    {
        var game = await gameService.GetGameByKeyAsync(gameKey, cancellationToken);
        if (game is null)
        {
            return NotFound($"Game with key {gameKey} not found.");
        }

        var gameFile = await gameFileService.CreateGameFileAsync(game);
        var memoryStream = new MemoryStream();
        await using (var stream = new FileStream(gameFile, FileMode.Open, FileAccess.Read))
        {
            await stream.CopyToAsync(memoryStream, cancellationToken);
        }

        memoryStream.Seek(0, SeekOrigin.Begin);
        var fileName = Path.GetFileName(gameFile);
        var streamResult = File(memoryStream, "application/octet-stream", fileName);
        return streamResult;
    }

    /// <summary>
    /// Returns game by id.
    /// </summary>
    /// <param name="id">ID to look for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Found game.</returns>
    [HttpGet("find/{id:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [Authorize(Policy = nameof(UserPermissionTypes.ViewGames))]
    public async Task<ActionResult<GameDto>> GetGameById(Guid id, CancellationToken cancellationToken)
    {
        var game = await gameService.GetGameByIdAsync(id, cancellationToken);
        return game is null ?
            NotFound($"Game with ID {id} could not be found.") :
            Ok(game);
    }

    /// <summary>
    /// Updates game in the database.
    /// </summary>
    /// <param name="gameUpdateRequestDto">Request with updated information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpPut]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [Authorize(Policy = nameof(UserPermissionTypes.ManageEntities))]
    public async Task<IActionResult> UpdateGame(GameUpdateRequestDto gameUpdateRequestDto, CancellationToken cancellationToken)
    {
        await gameService.UpdateGameAsync(gameUpdateRequestDto, cancellationToken);
        return NoContent();
    }
    
    [HttpGet("{gameKey}/image")]
    [ResponseCache(Duration = 64000)]
    public async Task<IActionResult> GetGameImage(string gameKey, CancellationToken cancellationToken)
    {
        var cacheKey = $"GameImage:{gameKey}";

        if (cache.TryGetValue<(byte[], string)>(cacheKey, out var cached))
        {
            return File(cached.Item1, cached.Item2);
        }

        var result = await gameService.GetImageByGameKeyAsync(gameKey, cancellationToken);
        if (result.Content.Length == 0)
        {
            return Ok();
        }

        cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));

        return File(result.Content, result.ContentType);
    }

    /// <summary>
    /// Adds game to the cart.
    /// </summary>
    /// <param name="gameKey">Game to add to the cart.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Ok if success, 404 if game not found.</returns>
    [HttpPost("{gameKey}/buy")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [Authorize(Policy = nameof(UserPermissionTypes.ViewGames))]
    public async Task<IActionResult> BuyGame(string gameKey, CancellationToken cancellationToken)
    {
        var user = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException());
        await orderService.AddGameToCartAsync(user, gameKey, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Adds comment to the game.
    /// </summary>
    /// <param name="gameKey">Game to add comment to.</param>
    /// <param name="commentCreateRequestDto">AddComment dto.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>201, 403.</returns>
    [HttpPost("{gameKey}/comments")]
    [ProducesResponseType(201)]
    [ProducesResponseType(404)]
    [Authorize(Policy = nameof(UserPermissionTypes.AddComments))]
    [Authorize(Policy = AuthorizationPolicies.NotBanned)]
    public async Task<IActionResult> AddComment(string gameKey, AddCommentRequestDto commentCreateRequestDto, CancellationToken cancellationToken)
    {
        var user = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException());
        
        await commentService.AddCommentAsync(user, commentCreateRequestDto, gameKey, cancellationToken);
        return CreatedAtAction(nameof(GetGameByKey), new { gameKey }, commentCreateRequestDto);
    }

    /// <summary>
    /// Gets all comments for the game.
    /// </summary>
    /// <param name="gameKey">Game key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of comments.</returns>
    [HttpGet("{gameKey}/comments")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [Authorize(Policy = nameof(UserPermissionTypes.ViewGames))]
    public async Task<ActionResult<IEnumerable<AddCommentDto>>> GetCommentsByGameKey(string gameKey, CancellationToken cancellationToken)
    {
        var comments = await commentService.GetCommentsByGameKeyAsync(gameKey, cancellationToken);
        return Ok(comments);
    }

    /// <summary>
    /// Deletes comment by id.
    /// </summary>
    /// <param name="gameKey">Key of the game.</param>
    /// <param name="commentId">Guid of the comment.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{gameKey}/comments/{commentId:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [Authorize(Policy = nameof(UserPermissionTypes.ManageComments))]
    public async Task<IActionResult> DeleteComment(string gameKey, Guid commentId, CancellationToken cancellationToken)
    {
        if (await gameService.GetGameByKeyAsync(gameKey, cancellationToken) is null)
        {
            return NotFound($"Comment with id {gameKey} not found.");
        }

        await commentService.DeleteCommentAsync(commentId, cancellationToken);
        return NoContent();
    }
    
    /// <summary>
    /// Method to get options for pagination
    /// </summary>
    /// <returns>Available options for pagination</returns>
    [Route("pagination-options")]
    [HttpGet]
    public IActionResult GetPaginationOptions()
    {  
        var options = Enum.GetValues(typeof(PaginationOptions))
            .Cast<PaginationOptions>()
            .Select(opt => opt.GetDescription())
            .ToList();
        return Ok(options);
    }

    /// <summary>
    /// Method to get options for sorting
    /// </summary>
    /// <returns>Available options for sorting</returns>
    [Route("sorting-options")]
    [HttpGet]
    public IActionResult GetSortingOptions()
    {
        var options = Enum.GetValues(typeof(SortingOptions))
            .Cast<SortingOptions>()
            .ToList();
        return Ok(options);
    }
    
    /// <summary>
    /// Method to get options for sorting
    /// </summary>
    /// <returns>Available options for sorting</returns>
    [Route("publish-date-options")]
    [HttpGet]
    public IActionResult GetPublishDateOptions()
    {
        var options = Enum.GetValues(typeof(PublishingDateFilter))
            .Cast<PublishingDateFilter>()
            .ToList();
        return Ok(options);
    }


    [HttpGet("test")]
    public async Task TestSysLogServer()
    {

        logger.LogInformation("Сторінка Index завантажена");
        logger.LogError("Приклад помилки");
        logger.LogCritical("Приклад помилки 2");
        logger.LogWarning("Приклад варнінга");
    }
}
