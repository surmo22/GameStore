using GameStore.BLL.DTOs.Role;
using GameStore.BLL.DTOs.Role.Create;
using GameStore.BLL.DTOs.Role.Update;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.Domain.Entities.Enums;
using GameStore.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Tests.ApiTests;

public class RolesControllerTests
{
    private readonly Mock<IRoleService> _roleServiceMock;
    private readonly RolesController _controller;

    public RolesControllerTests()
    {
        _roleServiceMock = new Mock<IRoleService>();
        _controller = new RolesController(_roleServiceMock.Object);
    }

    [Fact]
    public async Task GetAllRoles_ReturnsOkWithRoles()
    {
        // Arrange
        var roles = new List<RoleDto> { new() };
        _roleServiceMock.Setup(s => s.GetAllRolesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(roles);

        // Act
        var result = await _controller.GetAllRoles(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(roles, okResult.Value);
    }

    [Fact]
    public async Task GetRoleById_ReturnsOkWithRole()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var role = new  RoleDto { Id = roleId, Name = "Admin" };
        _roleServiceMock.Setup(s => s.GetRoleByIdAsync(roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        // Act
        var result = await _controller.GetRoleById(roleId, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(role, okResult.Value);
    }

    [Fact]
    public async Task DeleteRole_ReturnsOk()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        _roleServiceMock.Setup(s => s.DeleteRoleByIdAsync(roleId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteRole(roleId, CancellationToken.None);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public void GetPermissions_ReturnsOkWithPermissions()
    {
        // Act
        var result = _controller.GetPermissions();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var permissions = Assert.IsAssignableFrom<List<UserPermissionTypes>>(okResult.Value);

        Assert.Contains(UserPermissionTypes.ManageRoles, permissions);
    }

    [Fact]
    public async Task GetRolePermissions_ReturnsOkWithPermissions()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var permissions = new List<UserPermissionTypes> { UserPermissionTypes.ManageUsers };
        _roleServiceMock.Setup(s => s.GetRolePermissionsByRoleIdAsync(roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserPermissionTypes> { UserPermissionTypes.ManageUsers });

        // Act
        var result = await _controller.GetRolePermissions(roleId, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(permissions, okResult.Value);
    }

    [Fact]
    public async Task CreateRole_ReturnsOk()
    {
        // Arrange
        var createRoleDto = new CreateRoleDto { Role = new CreateRoleInfo{ Name = "NewRole"} };
        _roleServiceMock.Setup(s => s.CreateRole(createRoleDto, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.CreateRole(createRoleDto, CancellationToken.None);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task UpdateRole_ReturnsOk()
    {
        // Arrange
        var updateRoleDto = new UpdateRoleDto { Role = new UpdateRoleInfo(){ Id = Guid.NewGuid(), Name = "UpdatedRole"} };
        _roleServiceMock.Setup(s => s.UpdateRoleAsync(updateRoleDto, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateRole(updateRoleDto, CancellationToken.None);

        // Assert
        Assert.IsType<OkResult>(result);
    }
}