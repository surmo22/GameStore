using GameStore.BLL.DTOs.Role;
using GameStore.BLL.DTOs.Role.Create;
using GameStore.BLL.DTOs.Role.Update;
using GameStore.Domain.Entities.Enums;

namespace GameStore.BLL.Interfaces.EntityServices;

/// Provides operations related to managing roles in the system.
public interface IRoleService
{
    /// <summary>
    /// Asynchronously retrieves all roles.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of role data transfer objects.</returns>
    Task<List<RoleDto>> GetAllRolesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a role by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the role to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the role data as a <see cref="RoleDto"/>.</returns>
    Task<RoleDto> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a role by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the role to be deleted.</param>
    /// <param name="cancellationToken">Token used to propagate notifications that the operation should be canceled.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteRoleByIdAsync(Guid id, CancellationToken cancellationToken);

    /// Retrieves a list of permissions associated with a specific role by its ID.
    /// <param name="id">The unique identifier of the role.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task representing the asynchronous operation, containing a list of user permissions associated with the role.</return>
    Task<List<UserPermissionTypes>> GetRolePermissionsByRoleIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new role with the specified properties and assigns the provided permissions.
    /// </summary>
    /// <param name="createRoleDto">An object containing the role information and associated permissions.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateRole(CreateRoleDto createRoleDto, CancellationToken cancellationToken);

    /// Updates the information about an existing role and its associated permissions.
    /// <param name="updateRoleDto">
    /// An object containing the updated role information, including its ID, name, and associated permissions.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token that propagates notification that the operation should be canceled.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    Task UpdateRoleAsync(UpdateRoleDto updateRoleDto, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a list of role names corresponding to the provided role GUIDs.
    /// </summary>
    /// <param name="roles">A list of GUIDs representing the roles to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token to monitor for task cancellation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of role names.</returns>
    Task<List<string>> GetRoleNamesByGuidAsync(List<Guid> roles, CancellationToken cancellationToken);

    /// Retrieves a list of roles based on the specified role names.
    /// <param name="roleNames">A list of role names to search for.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of RoleDto objects that match the specified role names.</returns>
    Task<List<RoleDto>> GetRolesByNamesAsync(IList<string> roleNames, CancellationToken cancellationToken);
}