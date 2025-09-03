using System.Security.Claims;
using AutoMapper;
using GameStore.BLL.DTOs.Role;
using GameStore.BLL.DTOs.Role.Create;
using GameStore.BLL.DTOs.Role.Update;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.Common.Constants;
using GameStore.Common.Utils;
using GameStore.Domain.Entities.Enums;
using GameStore.Domain.Entities.UserEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameStore.BLL.Services.EntityServices;

public class RoleService(
    RoleManager<Role>    roleManager,
    IMapper              mapper,
    ILogger<RoleService> logger,
    IGuidProvider        guidProvider) : IRoleService
{
    public async Task<List<string>> GetRoleNamesByGuidAsync(List<Guid> roles, CancellationToken cancellationToken)
    {
        var assignedRoles = new List<string>();
        
        foreach (var roleId in roles)
        {
            var role = await roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
            {
                throw new InvalidOperationException($"Role '{roleId}' does not exist.");
            }

            assignedRoles.Add(role.Name!);
        }
        
        return assignedRoles;
    }

    public async Task<List<RoleDto>> GetRolesByNamesAsync(IList<string> roleNames, CancellationToken cancellationToken)
    {
        var roles = await roleManager.Roles.Where(r => roleNames.Contains(r.Name!)).ToListAsync(cancellationToken);
        var rolesDtos = mapper.Map<List<RoleDto>>(roles);
        return rolesDtos;
    }
    
    public async Task<List<RoleDto>> GetAllRolesAsync(CancellationToken cancellationToken)
    {
        var roles = await roleManager.Roles.ToListAsync(cancellationToken);
        var rolesDtos = mapper.Map<List<RoleDto>>(roles);
        return rolesDtos;
    }
    
    public async Task<RoleDto> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(id.ToString());
        var roleDto = mapper.Map<RoleDto>(role);
        return roleDto;
    }

    public async Task DeleteRoleByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(id.ToString()) ?? throw new KeyNotFoundException($"Role with id {id} not found");
        await roleManager.DeleteAsync(role);
        logger.LogInformation("Role with id {roleId} was deleted", role.Id);
    }

    public async Task<List<UserPermissionTypes>> GetRolePermissionsByRoleIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(id.ToString());
        if (role == null)
        {
            throw new KeyNotFoundException($"Role with ID {id} not found.");
        }
        
        var permissions = await roleManager.GetClaimsAsync(role);
        var permissionsNames = permissions.Select(p => p.Value)
            .Select(EnumHelper.ParseEnum<UserPermissionTypes>).ToList();
        return permissionsNames;
    }

    public async Task CreateRole(CreateRoleDto createRoleDto, CancellationToken cancellationToken)
    {
        var role = new Role()
        {
            Id = guidProvider.NewGuid(),
            Name = createRoleDto.Role.Name,
            IsSystemRole = false,
        };
        
        await roleManager.CreateAsync(role);

        foreach (var claim in createRoleDto.Permissions.Select(permission => new Claim(ClaimType.Permission, permission.ToString())))
        {
            await roleManager.AddClaimAsync(role, claim);
        }
        
        logger.LogInformation("New role with id {roleId} was created", role.Id);
    }

    public async Task UpdateRoleAsync(UpdateRoleDto updateRoleDto, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(updateRoleDto.Role.Id.ToString());
        if (role is null)
        {
            throw new KeyNotFoundException($"Role with ID {updateRoleDto.Role.Id} not found.");
        }

        role.Name = updateRoleDto.Role.Name;
        
        var claimsToRemove = await roleManager.GetClaimsAsync(role);
        
        foreach (var claim in claimsToRemove)
        {
            await roleManager.RemoveClaimAsync(role, claim);
        }
        
        foreach (var claim in updateRoleDto.Permissions.Select(permission => new Claim(ClaimType.Permission, permission.ToString())))
        {
            await roleManager.AddClaimAsync(role, claim);
        }
    }
}