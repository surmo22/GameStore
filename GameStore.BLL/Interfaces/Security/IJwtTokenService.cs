using System.Security.Claims;

namespace GameStore.BLL.Interfaces.Security;

/// <summary>
/// Represents a service for creating and managing JSON Web Tokens (JWT).
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates a JSON Web Token (JWT) access token containing the provided claims.
    /// </summary>
    /// <param name="claims">A collection of claims to be included in the token.</param>
    /// <returns>A string representing the generated JWT access token.</returns>
    public string GenerateAccessToken(IEnumerable<Claim> claims);
}