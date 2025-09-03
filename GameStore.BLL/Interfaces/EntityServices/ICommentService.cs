using GameStore.BLL.DTOs.Comment;

namespace GameStore.BLL.Interfaces.EntityServices;

/// <summary>
/// Defines the contract for managing comments within the application.
/// </summary>
public interface ICommentService
{
    /// <summary>
    /// Adds a new comment to a game.
    /// </summary>
    /// <param name="user">The ID of the user adding the comment.</param>
    /// <param name="addCommentRequestDto">The comment data and related information.</param>
    /// <param name="gameKey">The unique key of the game to which the comment is being added.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddCommentAsync(Guid user, AddCommentRequestDto addCommentRequestDto, string gameKey,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a collection of comments associated with a specific game, identified by its game key.
    /// </summary>
    /// <param name="gameKey">The unique identifier (key) of the game.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a collection of <see cref="GetCommentDto"/> representing the comments related to the specified game.</returns>
    Task<IEnumerable<GetCommentDto>> GetCommentsByGameKeyAsync(string gameKey, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the comment with the specified unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the comment to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the comment details as a <see cref="GetCommentDto"/> object.</returns>
    Task<GetCommentDto> GetCommentByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a comment identified by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the comment to be deleted.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteCommentAsync(Guid id, CancellationToken cancellationToken);
}