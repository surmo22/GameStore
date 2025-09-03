using System.Security.Claims;

namespace GameStore.BLL.Interfaces.Security;

/// Interface for retrieving and managing the current user's context and permissions.
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user's identity and claims information.
    /// </summary>
    /// <remarks>
    /// The property returns an instance of <see cref="ClaimsPrincipal"/>,
    /// which represents the current user's authentication context, including
    /// identity and claims.
    /// </remarks>
    ClaimsPrincipal User { get; }

    /// <summary>
    /// Gets the unique identifier of the current user.
    /// </summary>
    /// <remarks>
    /// The property retrieves the user's identifier as a <see cref="Guid"/> based on the
    /// claim of type <see cref="ClaimTypes.NameIdentifier"/>. If the claim is not present
    /// or cannot be parsed, the property defaults to <see cref="Guid.Empty"/>.
    /// </remarks>
    Guid UserId { get; }

    /// Checks whether the current user has the specified permission asynchronously.
    /// <param name="permission">
    ///     The permission to check for, represented as a string. For example, this could be "ManageUsers" or "ViewOrderHistory".
    /// </param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used to propagate notification that the operation should be canceled.
    /// </param>
    /// <return>
    /// A task representing the asynchronous operation. The task result contains a boolean value indicating whether the user has the specified permission.
    /// </return>
    Task<bool> HasPermissionToAsync(string permission, CancellationToken cancellationToken);
}