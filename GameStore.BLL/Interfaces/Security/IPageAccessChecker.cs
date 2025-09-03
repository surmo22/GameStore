using GameStore.BLL.DTOs.User;

namespace GameStore.BLL.Interfaces.Security;

/// <summary>
/// Interface for handling page access validation within the application.
/// </summary>
public interface IPageAccessChecker
{
    /// <summary>
    /// Checks if access to a specified page and target ID is allowed based on the implemented access rules.
    /// </summary>
    /// <param name="request">The access request details containing the target page and an optional target ID.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a boolean indicating whether access is allowed.</returns>
    Task<bool> CheckAccessAsync(CheckAccessRequestDto request, CancellationToken cancellationToken);
}