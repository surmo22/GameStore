using GameStore.BLL.DTOs.Role.Create;
using GameStore.BLL.DTOs.Role.Update;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.Domain.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize(Policy = nameof(UserPermissionTypes.ManageRoles))]
public class RolesController(IRoleService roleService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllRoles(CancellationToken cancellationToken)
    {
        var roles = await roleService.GetAllRolesAsync(cancellationToken);
        return Ok(roles);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetRoleById(Guid id, CancellationToken cancellationToken)
    {
        var role = await roleService.GetRoleByIdAsync(id, cancellationToken);
        return Ok(role);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteRole(Guid id, CancellationToken cancellationToken)
    {
        await roleService.DeleteRoleByIdAsync(id, cancellationToken);
        return Ok();
    }

    [HttpGet("permissions")]
    public IActionResult GetPermissions()
    {
        var result = Enum.GetValues(typeof(UserPermissionTypes))
            .Cast<UserPermissionTypes>()
            .ToList();
        return Ok(result);
    }

    [HttpGet("{id:guid}/permissions")]
    public async Task<IActionResult> GetRolePermissions(Guid id, CancellationToken cancellationToken)
    {
        var result = await roleService.GetRolePermissionsByRoleIdAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole(CreateRoleDto createRoleDto, CancellationToken cancellationToken)
    {
        await roleService.CreateRole(createRoleDto, cancellationToken);
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateRole(UpdateRoleDto updateRoleDto, CancellationToken cancellationToken)
    {
        await roleService.UpdateRoleAsync(updateRoleDto, cancellationToken);
        return Ok();
    }
}