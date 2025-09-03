using GameStore.BLL.DTOs.User.Login;
using GameStore.BLL.Interfaces.Security;
using GameStore.Domain.Entities.UserEntities;
using Microsoft.AspNetCore.Identity;

namespace GameStore.BLL.Services.Security.AuthentificationStrategies;

public class InternalAuthentificationStrategy(UserManager<User> userManager) : IAuthentificationStrategy
{
    public async Task<User> AuthenticateAsync(LoginRequestDto loginRequestDto, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByNameAsync(loginRequestDto.Model.Login);
        if (user == null || !await userManager.CheckPasswordAsync(user, loginRequestDto.Model.Password))
        {
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        return user;
    }

    public bool CanAuthenticate(LoginRequestDto loginRequestDto)
    {
        return loginRequestDto.Model.InternalAuth;
    }
}