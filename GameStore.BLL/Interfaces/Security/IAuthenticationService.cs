using GameStore.BLL.DTOs.User.Login;

namespace GameStore.BLL.Interfaces.Security;

/// <summary>
/// Defines methods for authenticating and authorizing users within the application.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Authenticates a user based on the provided login request and generates a token for accessing authorized resources.
    /// </summary>
    /// <param name="request">The login request containing user credentials.</param>
    /// <param name="cancellationToken">The token used to cancel the authentication process.</param>
    /// <returns>A <see cref="LoginResponseDto"/> containing the generated access token for the authenticated user.</returns>
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken);
}