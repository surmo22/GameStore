using System.Net.Http.Json;
using GameStore.BLL.DTOs.User.Login;
using GameStore.Domain.Entities.Enums;
using GameStore.Domain.Entities.UserEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace GameStore.IntegrationTests.Helpers;

public static class Authenticator
{
    public static async Task<Guid> AuthenticateClientAsync(HttpClient client, IServiceProvider serviceProvider)
    {
        var userId = await SeedUser(serviceProvider);
        var loginRequest = new LoginRequestDto
        {
            Model = new()
            {
                Login = "admin",
                Password = "admin",
                InternalAuth = true,
            },
        };

        var response = await client.PostAsJsonAsync("users/login", loginRequest);
        response.EnsureSuccessStatusCode();

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        var token = loginResponse?.Token;
        var rawToken = token?.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", rawToken);
        return userId;
    }
    
    private static async Task<Guid> SeedUser(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            UserName = "admin",
            Email = "testuser@example.com",
            UserNotificationTypes = [UserNotificationTypes.Email, UserNotificationTypes.Push, UserNotificationTypes.Sms]
        };
        await userManager.CreateAsync(user, "admin");
        await userManager.AddToRoleAsync(user, "User");
        await userManager.AddToRoleAsync(user, "Manager");
        await userManager.AddToRoleAsync(user, "Moderator");
        await userManager.AddToRoleAsync(user, "Administrator");

        var user2 = new User
        {
            Id = Guid.NewGuid(),
            UserName = "gigatest",
            UserNotificationTypes = [UserNotificationTypes.Email, UserNotificationTypes.Push, UserNotificationTypes.Sms]
        };
        await userManager.CreateAsync(user2, "gigatest");
        await userManager.AddToRoleAsync(user2, "User");
        return userId;
    }

    public static async Task SeedUserAsync(User user, HttpClient httpClient, IServiceProvider serviceProvider)
    {
        user.UserNotificationTypes =
            [UserNotificationTypes.Email, UserNotificationTypes.Push, UserNotificationTypes.Sms];
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        await userManager.CreateAsync(user, "admin");
        await userManager.AddToRoleAsync(user, "Administrator");
        
        var loginRequest = new LoginRequestDto
        {
            Model = new()
            {
                Login = user.UserName!,
                Password = "admin",
                InternalAuth = true,
            },
        };
        
        var response = await httpClient.PostAsJsonAsync("users/login", loginRequest);
        response.EnsureSuccessStatusCode();

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        var token = loginResponse?.Token;
        var rawToken = token?.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", rawToken);
    }
}