namespace GameStore.BLL.Interfaces.Security;

/// Represents a rule for determining access to a particular page or resource.
public interface IPageAccessRule
{
    /// Determines whether the current rule is responsible for handling access control for the specified page.
    /// <param name="pageKey">The key of the page to check access for.</param>
    /// <returns>True if the rule can handle the specified page; otherwise, false.</returns>
    bool CanHandle(string pageKey);

    /// Determines whether access is permitted to a specific resource or page for the current user.
    /// <param name="targetId">The identifier of the resource or page to check access for.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains true if access is allowed, otherwise false.</returns>
    Task<bool> HasAccessAsync(string? targetId, CancellationToken cancellationToken);
}