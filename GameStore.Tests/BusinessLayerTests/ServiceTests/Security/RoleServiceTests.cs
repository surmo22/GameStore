using System.Security.Claims;
using AutoMapper;
using FluentAssertions;
using GameStore.BLL.DTOs.Role;
using GameStore.BLL.DTOs.Role.Create;
using GameStore.BLL.DTOs.Role.Update;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Services.EntityServices;
using GameStore.Common.Constants;
using GameStore.Domain.Entities.Enums;
using GameStore.Domain.Entities.UserEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.Security;


public class RoleServiceTests
{
    private readonly Mock<RoleManager<Role>> _roleManagerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IGuidProvider> _guidProvider = new();
    private readonly RoleService _roleService;

    public RoleServiceTests()
    {
        var roleStoreMock = new Mock<IRoleStore<Role>>();
        _roleManagerMock = new Mock<RoleManager<Role>>(roleStoreMock.Object, null, null, null, null);
        _mapperMock = new Mock<IMapper>();
        _guidProvider.Setup(x => x.NewGuid()).Returns(Guid.NewGuid());
        var loggerMock = new Mock<ILogger<RoleService>>();
        _roleService = new RoleService(_roleManagerMock.Object, _mapperMock.Object, loggerMock.Object,
            _guidProvider.Object);
    }

    [Fact]
    public async Task GetRoleNamesByGuidAsync_ValidRoleIds_ReturnsRoleNames()
    {
        // Arrange
        var roleIds = new List<Guid> { Guid.NewGuid() };
        var role = new Role { Id = roleIds[0], Name = "TestRole" };
        _roleManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(role);

        // Act
        var result = await _roleService.GetRoleNamesByGuidAsync(roleIds, CancellationToken.None);

        // Assert
        result.Should().ContainSingle();
        result[0].Should().Be("TestRole");
    }

    [Fact]
    public async Task GetRolesByNamesAsync_ValidRoleNames_ReturnsRoleDtos()
    {
        // Arrange
        var roleNames = new List<string> { "TestRole" };
        var roles = new List<Role> { new() { Name = "TestRole" } };
        var roleDtos = new List<RoleDto> { new() { Name = "TestRole" } };

        _roleManagerMock.Setup(x => x.Roles)
            .Returns(roles.BuildMock());
        _mapperMock.Setup(x => x.Map<List<RoleDto>>(It.IsAny<List<Role>>()))
            .Returns(roleDtos);

        // Act
        var result = await _roleService.GetRolesByNamesAsync(roleNames, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("TestRole");
    }

    [Fact]
    public async Task GetAllRolesAsync_ShouldReturnAllRoles()
    {
        // Arrange
        var roles = new List<Role> { new() { Name = "TestRole" } };
        var roleDtos = new List<RoleDto> { new() { Name = "TestRole" } };

        _roleManagerMock.Setup(x => x.Roles)
            .Returns(roles.BuildMock());
        _mapperMock.Setup(x => x.Map<List<RoleDto>>(It.IsAny<List<Role>>()))
            .Returns(roleDtos);

        // Act
        var result = await _roleService.GetAllRolesAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("TestRole");
    }

    [Fact]
    public async Task GetRoleByIdAsync_ValidId_ReturnsRoleDto()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var role = new Role { Id = roleId, Name = "TestRole" };
        var roleDto = new RoleDto { Id = roleId, Name = "TestRole" };

        _roleManagerMock.Setup(x => x.FindByIdAsync(roleId.ToString()))
            .ReturnsAsync(role);
        _mapperMock.Setup(x => x.Map<RoleDto>(role))
            .Returns(roleDto);

        // Act
        var result = await _roleService.GetRoleByIdAsync(roleId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(roleId);
        result.Name.Should().Be("TestRole");
    }

    [Fact]
    public async Task DeleteRoleByIdAsync_ValidId_DeletesRole()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var role = new Role { Id = roleId, Name = "TestRole" };

        _roleManagerMock.Setup(x => x.FindByIdAsync(roleId.ToString()))
            .ReturnsAsync(role);
        _roleManagerMock.Setup(x => x.DeleteAsync(role))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await _roleService.DeleteRoleByIdAsync(roleId, CancellationToken.None);

        // Assert
        _roleManagerMock.Verify(x => x.DeleteAsync(role), Times.Once);
    }

    [Fact]
    public async Task GetRolePermissionsByRoleIdAsync_ValidId_ReturnsPermissions()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var role = new Role { Id = roleId, Name = "TestRole" };
        var claims = new List<Claim> { new(ClaimType.Permission, nameof(UserPermissionTypes.AddComments)) };

        _roleManagerMock.Setup(x => x.FindByIdAsync(roleId.ToString()))
            .ReturnsAsync(role);
        _roleManagerMock.Setup(x => x.GetClaimsAsync(role))
            .ReturnsAsync(claims);

        // Act
        var result = await _roleService.GetRolePermissionsByRoleIdAsync(roleId, CancellationToken.None);

        // Assert
        result.Should().ContainSingle();
        result[0].Should().Be(UserPermissionTypes.AddComments);
    }

    [Fact]
    public async Task CreateRole_ValidRole_CreatesRole()
    {
        // Arrange
        var createRoleDto = new CreateRoleDto
        {
            Role = new CreateRoleInfo() { Name = "TestRole" },
            Permissions = new List<UserPermissionTypes> { UserPermissionTypes.AddComments }
        };

        _roleManagerMock.Setup(x => x.CreateAsync(It.IsAny<Role>()))
            .ReturnsAsync(IdentityResult.Success);
        _roleManagerMock.Setup(x => x.AddClaimAsync(It.IsAny<Role>(), It.IsAny<Claim>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await _roleService.CreateRole(createRoleDto, CancellationToken.None);

        // Assert
        _roleManagerMock.Verify(x => x.CreateAsync(It.IsAny<Role>()), Times.Once);
        _roleManagerMock.Verify(x => x.AddClaimAsync(It.IsAny<Role>(), It.IsAny<Claim>()), Times.Once);
    }

    [Fact]
    public async Task UpdateRoleAsync_ValidRole_UpdatesRole()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var updateRoleDto = new UpdateRoleDto
        {
            Role = new UpdateRoleInfo() { 
                Id = Guid.NewGuid(),
                Name = "TestRole"
            },
            Permissions = new List<UserPermissionTypes> { UserPermissionTypes.AddComments }
        };
        var role = new Role { Id = roleId, Name = "TestRole" };
        var existingClaims = new List<Claim> { new(ClaimType.Permission, "TestPermission") };

        _roleManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(role);
        _roleManagerMock.Setup(x => x.GetClaimsAsync(role))
            .ReturnsAsync(existingClaims);
        _roleManagerMock.Setup(x => x.RemoveClaimAsync(role, It.IsAny<Claim>()))
            .ReturnsAsync(IdentityResult.Success);
        _roleManagerMock.Setup(x => x.AddClaimAsync(role, It.IsAny<Claim>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await _roleService.UpdateRoleAsync(updateRoleDto, CancellationToken.None);

        // Assert
        role.Name.Should().Be("TestRole");
        _roleManagerMock.Verify(x => x.RemoveClaimAsync(role, It.IsAny<Claim>()), Times.Once);
        _roleManagerMock.Verify(x => x.AddClaimAsync(role, It.IsAny<Claim>()), Times.Once);
    }
}
