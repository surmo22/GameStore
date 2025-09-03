using AutoMapper;
using GameStore.BLL.DTOs.Role;
using GameStore.BLL.DTOs.User;
using GameStore.BLL.DTOs.User.Creation;
using GameStore.BLL.DTOs.User.Update;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.Common.Constants;
using GameStore.Domain.Entities.Enums;
using GameStore.Domain.Entities.UserEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameStore.BLL.Services.EntityServices;

public class UserService(
    UserManager<User>    userManager,
    IMapper              mapper,
    ILogger<UserService> logger,
    IRoleService         roleService) : IUserService
{
    public async Task CreateUserAsync(CreateUserDto createUserDto, CancellationToken cancellationToken)
    {
        if (await userManager.FindByNameAsync(createUserDto.User.Name) != null)
        {
            throw new InvalidOperationException("User already exists");
        }

        var user = new User
        {
            UserName = createUserDto.User.Name,
            Email = createUserDto.User.Name,
            IsActive = true,
            UserNotificationTypes = [UserNotificationTypes.Email, UserNotificationTypes.Push, UserNotificationTypes.Sms]
        };

        var result = await userManager.CreateAsync(user, createUserDto.Password);
        
        if (!result.Succeeded)
        {
            logger.LogError("Failed to create user");
            foreach (var error in result.Errors)
            {
                logger.LogError("User creation error: {Error}", error.Description);
            }
            throw new InvalidOperationException("Failed to create user");
        }
        
        var assignedRoles = await roleService.GetRoleNamesByGuidAsync(createUserDto.Roles, cancellationToken);
        
        await userManager.AddToRolesAsync(user, assignedRoles);
        logger.LogInformation("New user with id {userId} was created", user.Id);
    }

    public async Task CreateUserWithoutPasswordAsync(string email, string name,
        CancellationToken cancellationToken)
    {
        await userManager.CreateAsync(new User
        {
            Email = email,
            UserName = name,
        });

        var user = await userManager.FindByNameAsync(name) ??
                   throw new UnauthorizedAccessException("Failed to create user");
        await userManager.AddToRoleAsync(user , nameof(DefaultRoles.User));
    }

    public async Task<UserDto> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(email);
        var userDto = mapper.Map<UserDto>(user);
        return userDto;
    }

    public async Task<Guid> GetUserIdByUserNameAsync(string userName)
    {
        var user = await userManager.FindByNameAsync(userName);
        return user.Id;
    }

    public async Task<List<UserDto>> GetAllUsers(CancellationToken cancellationToken)
    {
        var users = await userManager.Users.ToListAsync(cancellationToken);
        var usersDtos = users.Select(mapper.Map<UserDto>).ToList();
        return usersDtos;
    }
    
    public async Task<UserDto> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        var userDto = mapper.Map<UserDto>(user);
        return userDto;
    }
    
    public async Task DeleteUserById(Guid id, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(id.ToString()) ?? throw new KeyNotFoundException($"User with id {id} not found");
        await userManager.DeleteAsync(user);
        logger.LogInformation("User with id {userId} was deleted", user.Id);
    }

    public async Task<List<RoleDto>> GetUserRolesByUserId(Guid id, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }

        var roleNames = await userManager.GetRolesAsync(user);

        var roles = await roleService.GetRolesByNamesAsync(roleNames, cancellationToken);

        return roles;
    }

    public async Task UpdateUserAsync(UpdateUserDto updateUserDto, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(updateUserDto.User.Id.ToString());
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {updateUserDto.User.Id} not found.");
        }
        
        user.UserName = updateUserDto.User.Name;
        user.Email = updateUserDto.User.Name;
        

        var assignedRoles = await roleService.GetRoleNamesByGuidAsync(updateUserDto.Roles, cancellationToken);
        
        
        await userManager.RemoveFromRolesAsync(user, await userManager.GetRolesAsync(user));
        await userManager.AddToRolesAsync(user, assignedRoles);
        await userManager.UpdateAsync(user);
        await userManager.RemovePasswordAsync(user);
        await userManager.AddPasswordAsync(user, updateUserDto.Password);
        
        logger.LogInformation("User with id {userId} was updated", user.Id);
    }
}