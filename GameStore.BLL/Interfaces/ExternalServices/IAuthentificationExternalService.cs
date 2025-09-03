using GameStore.BLL.Services.ExternalServices.Requests;
using GameStore.BLL.Services.ExternalServices.Responses;

namespace GameStore.BLL.Interfaces.ExternalServices;

/// <summary>
/// Defines the contract for external authentication services.
/// </summary>
public interface IAuthentificationExternalService
{
    /// <summary>
    /// Authenticates a user based on the provided request data and returns the authentication response.
    /// </summary>
    /// <param name="request">The authentication request containing the necessary data for user authentication.</param>
    /// <param name="cancellationToken">A cancellation token to observe for task cancellation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the authentication response.</returns>
    Task<AuthentificationResponse> Authenticate(AuthentificationRequest request, CancellationToken cancellationToken);
}