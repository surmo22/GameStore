using GameStore.BLL.DTOs.Role;
using GameStore.BLL.DTOs.User;
using GameStore.BLL.DTOs.User.Creation;
using GameStore.BLL.DTOs.User.Update;


namespace GameStore.BLL.Interfaces.EntityServices;

/// <summary>
/// Represents an interface for managing users in the system. Provides methods for creating, retrieving,
/// updating, deleting, and managing roles associated with users.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Asynchronously creates a new user in the system with the specified details and assigned roles.
    /// </summary>
    /// <param name="createUserDto">An object containing the user's information, roles, and password.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateUserAsync(CreateUserDto createUserDto, CancellationToken cancellationToken);

    /// Retrieves a list of all users in the system.
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task that represents the asynchronous operation. The task result contains a list of UserDto objects representing all users.</return>
    Task<List<UserDto>> GetAllUsers(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="UserDto"/> representing the user data, or null if the user is not found.</returns>
    Task<UserDto> GetUserById(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a user from the system based on their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    Task DeleteUserById(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the information of an existing user, including their roles and password.
    /// </summary>
    /// <param name="updateUserDto">An object containing the updated information for the user, including their basic info, roles, and password.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateUserAsync(UpdateUserDto updateUserDto, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the roles assigned to a user given their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A list of roles associated with the user.</returns>
    Task<List<RoleDto>> GetUserRolesByUserId(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new user in the system without setting a password.
    /// </summary>
    /// <param name="email">The email address of the user to be created.</param>
    /// <param name="name">The name of the user to be created.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task CreateUserWithoutPasswordAsync(string email, string name,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a user by their email address asynchronously.
    /// </summary>
    /// <param name="email">The email address of the user to retrieve.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="UserDto"/> object representing the user associated with the given email address.</returns>
    Task<UserDto> GetUserByEmailAsync(string email, CancellationToken cancellationToken);

    /// Retrieves the unique identifier (ID) of a user based on their username.
    /// <param name="userName">The username of the user whose ID is to be retrieved.</param>
    /// <returns>The unique identifier (ID) of the user.</returns>
    Task<Guid> GetUserIdByUserNameAsync(string userName);
}