namespace GameStore.BLL.Interfaces.Security;

/// Service interface for managing and verifying permissions of the current user.
public interface ICurrentUserPermissionService
{
    /// <summary>
    /// Determines whether the user with the specified ID has a specific permission.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="permission">The permission to check for the user.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean value indicating whether the user has the specified permission.</returns>
    Task<bool> HasPermissionAsync(Guid id, string permission, CancellationToken cancellationToken);
}