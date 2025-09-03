using GameStore.BLL.DTOs.Comment;

namespace GameStore.BLL.Interfaces.EntityServices;

/// <summary>
/// Provides operations related to managing user bans in the system.
/// </summary>
public interface IUserBanService
{
    /// <summary>
    /// Checks whether a user is banned based on their unique identifier (GUID).
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the user to check.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean value indicating whether the user is banned.</returns>
    Task<bool> IsUserBannedAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Bans a user by their username for the duration specified in the given request.
    /// </summary>
    /// <param name="userName">The username of the user to be banned.</param>
    /// <param name="request">The details of the ban, including the duration.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task BanUserAsync(string userName, BanUserRequest request, CancellationToken cancellationToken);
}