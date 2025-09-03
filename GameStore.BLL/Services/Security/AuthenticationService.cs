using System.Security.Claims;
using GameStore.BLL.DTOs.User.Login;
using GameStore.BLL.Interfaces.Security;
using GameStore.Domain.Entities.UserEntities;
using Microsoft.AspNetCore.Identity;

namespace GameStore.BLL.Services.Security;

public class AuthenticationService(
    UserManager<User>                      userManager,
    IJwtTokenService                       jwtTokenService,
    IEnumerable<IAuthentificationStrategy> authentificationStrategies) : IAuthenticationService
{
    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken)
    {
        var user = await authentificationStrategies.FirstOrDefault(s => s.CanAuthenticate(request))?
            .AuthenticateAsync(request, cancellationToken) ?? throw new UnauthorizedAccessException("Invalid username or password");

        var userRoles = await userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName!),
        };
        claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));
        
        var accessToken = jwtTokenService.GenerateAccessToken(claims);

        return new LoginResponseDto()
        {
            Token = $"Bearer {accessToken}",
        };
    }
}