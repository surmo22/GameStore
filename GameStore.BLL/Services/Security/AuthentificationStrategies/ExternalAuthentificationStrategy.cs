using GameStore.BLL.DTOs.User.Login;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.BLL.Interfaces.ExternalServices;
using GameStore.BLL.Interfaces.Security;
using GameStore.BLL.Services.ExternalServices.Requests;
using GameStore.Domain.Entities.UserEntities;
using Microsoft.AspNetCore.Identity;

namespace GameStore.BLL.Services.Security.AuthentificationStrategies;

public class ExternalAuthentificationStrategy(
    UserManager<User>                userManager,
    IAuthentificationExternalService externalAuthService,
    IUserService                     userService) : IAuthentificationStrategy
{
    public async Task<User> AuthenticateAsync(LoginRequestDto loginRequestDto, CancellationToken cancellationToken)
    {
        var request = new AuthentificationRequest()
        {
            Email = loginRequestDto.Model.Login,
            Password = loginRequestDto.Model.Password,
        };
        
        var response = await externalAuthService.Authenticate(request, cancellationToken);
        var existingUser = await userManager.FindByEmailAsync(response.Email);
        
        if (existingUser != null)
        {
            return existingUser;
        }
        
        await userService.CreateUserWithoutPasswordAsync(response.Email, response.FirstName, cancellationToken);
        
        var user = await userManager.FindByEmailAsync(response.Email);
        return user;
    }

    public bool CanAuthenticate(LoginRequestDto loginRequestDto)
    {
        return !loginRequestDto.Model.InternalAuth;
    }
}