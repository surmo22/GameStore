using GameStore.BLL.DTOs.User.Login;
using GameStore.Domain.Entities.UserEntities;

namespace GameStore.BLL.Interfaces.Security;

/// <summary>
/// Represents a strategy interface for user authentication.
/// Different implementations can provide various authentication mechanisms, such as internal or external authentication.
/// </summary>
public interface IAuthentificationStrategy
{
    /// <summary>
    /// Authenticates a user based on the provided login request data and cancellation token.
    /// </summary>
    /// <param name="loginRequestDto">The login request data containing the user's login credentials.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="User"/> object representing the authenticated user.</returns>
    Task<User> AuthenticateAsync(LoginRequestDto loginRequestDto, CancellationToken cancellationToken);

    /// <summary>
    /// Determines whether the strategy can authenticate the provided login request data.
    /// </summary>
    /// <param name="loginRequestDto">The login request data containing the user's login credentials.</param>
    /// <returns>A boolean value indicating whether the strategy supports authenticating the provided login request data.</returns>
    bool CanAuthenticate(LoginRequestDto loginRequestDto);
}