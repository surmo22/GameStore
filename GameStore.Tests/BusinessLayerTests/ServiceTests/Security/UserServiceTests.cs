using AutoMapper;
using FluentAssertions;
using GameStore.BLL.DTOs.Role;
using GameStore.BLL.DTOs.User;
using GameStore.BLL.DTOs.User.Creation;
using GameStore.BLL.DTOs.User.Update;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.BLL.Services.EntityServices;
using GameStore.Domain.Entities.UserEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using Xunit;

namespace GameStore.Tests.BusinessLayerTests.ServiceTests.Security;

public class UserServiceTests
{
    private readonly Mock<UserManager<User>> _userManager;
    private readonly Mock<IMapper> _mapper;
    private readonly Mock<IRoleService> _roleService;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        var userStoreMock = new Mock<IUserStore<User>>();

        _userManager = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
        _mapper = new Mock<IMapper>();
        var logger = new Mock<ILogger<UserService>>();
        _roleService = new Mock<IRoleService>();

        _sut = new UserService(_userManager.Object, _mapper.Object, logger.Object, _roleService.Object);
    }

    [Fact]
    public async Task CreateUserAsync_ThrowsException_WhenUserAlreadyExists()
    {
        // Arrange
        var createUserDto = new CreateUserDto { User = new UserInfo() { Name = "existingUser" } };
        _userManager.Setup(x => x.FindByNameAsync(createUserDto.User.Name))
            .ReturnsAsync(new User());

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _sut.CreateUserAsync(createUserDto, CancellationToken.None));
    }

    [Fact]
    public async Task GetUserByEmailAsync_ReturnsUserDto_WhenUserExists()
    {
        // Arrange
        var email = "test@test.com";
        var user = new User { Email = email, UserName = email};
        var expectedUserDto = new UserDto { Name = email };
        
        _userManager.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync(user);
        _mapper.Setup(x => x.Map<UserDto>(user)).Returns(expectedUserDto);

        // Act
        var result = await _sut.GetUserByEmailAsync(email, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedUserDto);
    }

    [Fact]
    public async Task DeleteUserById_ThrowsException_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userManager.Setup(x => x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync((User)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _sut.DeleteUserById(userId, CancellationToken.None));
    }

    [Fact]
    public async Task GetAllUsers_ReturnsUserDtoList()
    {
        // Arrange
        var users = new List<User> 
        { 
            new() { Id = Guid.NewGuid(), UserName = "user1" },
            new() { Id = Guid.NewGuid(), UserName = "user2" }
        };
        var userDtos = users.Select(u => new UserDto { Id = u.Id, Name = u.UserName! }).ToList();

        var dbSetMock = users.BuildMock();
        _userManager.Setup(x => x.Users).Returns(dbSetMock);
        _mapper.Setup(x => x.Map<UserDto>(It.IsAny<User>()))
            .Returns<User>(u => userDtos.First(dto => dto.Id == u.Id));

        // Act
        var result = await _sut.GetAllUsers(CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(userDtos);
    }
    
    [Fact]
    public async Task CreateUserAsync_SuccessfullyCreatesUserWithRoles()
    {
        // Arrange
        var createUserDto = new CreateUserDto
        {
            User = new UserInfo() { Name = "email@gamil.com" },
            Password = "Password123!",
            Roles = new List<Guid> { Guid.NewGuid() }
        };

        var expectedRoles = new List<string> { "Admin" };

        _userManager.Setup(x => x.FindByNameAsync(createUserDto.User.Name))
            .ReturnsAsync((User)null);

        _userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), createUserDto.Password))
            .ReturnsAsync(IdentityResult.Success);

        _roleService.Setup(x => x.GetRoleNamesByGuidAsync(createUserDto.Roles, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedRoles);

        _userManager.Setup(x => x.AddToRolesAsync(It.IsAny<User>(), expectedRoles))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await _sut.CreateUserAsync(createUserDto, CancellationToken.None);

        // Assert
        _userManager.Verify(x => x.CreateAsync(
                It.Is<User>(u => 
                    u.UserName == createUserDto.User.Name && 
                    u.IsActive == true), 
                createUserDto.Password), 
            Times.Once);
        _userManager.Verify(x => x.AddToRolesAsync(
                It.Is<User>(u => u.UserName == createUserDto.User.Name),
                expectedRoles), 
            Times.Once);
    }

    [Fact]
    public async Task GetUserIdByUserNameAsync_ReturnsUserId_WhenUserExists()
    {
        // Arrange
        var userName = "testuser";
        var expectedId = Guid.NewGuid();
        var user = new User { Id = expectedId, UserName = userName };

        _userManager.Setup(x => x.FindByNameAsync(userName))
            .ReturnsAsync(user);

        // Act
        var result = await _sut.GetUserIdByUserNameAsync(userName);

        // Assert
        Assert.Equal(expectedId, result);
    }

    [Fact]
    public async Task GetUserById_ReturnsUserDto_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, UserName = "testuser" };
        var expectedUserDto = new UserDto { Id = userId, Name = "testuser" };

        _userManager.Setup(x => x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);
        _mapper.Setup(x => x.Map<UserDto>(user))
            .Returns(expectedUserDto);

        // Act
        var result = await _sut.GetUserById(userId, CancellationToken.None);

        // Assert
        Assert.Equal(expectedUserDto.Id, result.Id);
        Assert.Equal(expectedUserDto.Name, result.Name);
    }
    
    [Fact]
    public async Task DeleteUserById_DeletesUser_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, UserName = "testuser" };

        _userManager.Setup(x => x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);
        _userManager.Setup(x => x.DeleteAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await _sut.DeleteUserById(userId, CancellationToken.None);

        // Assert
        _userManager.Verify(x => x.DeleteAsync(user), Times.Once);
    }
    
    [Fact]
    public async Task GetUserRolesByUserId_ReturnsRoles_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, UserName = "testuser" };
        var roleNames = new List<string> { "Admin", "User" };
        var expectedRoles = new List<RoleDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Admin" },
            new() { Id = Guid.NewGuid(), Name = "User" }
        };

        _userManager.Setup(x => x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);
        _userManager.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(roleNames);
        _roleService.Setup(x => x.GetRolesByNamesAsync(roleNames, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedRoles);

        // Act
        var result = await _sut.GetUserRolesByUserId(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedRoles.Count, result.Count);
        Assert.Equal(expectedRoles, result);
        _userManager.Verify(x => x.FindByIdAsync(userId.ToString()), Times.Once);
        _userManager.Verify(x => x.GetRolesAsync(user), Times.Once);
        _roleService.Verify(x => x.GetRolesByNamesAsync(roleNames, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task CreateUserAsync_ThrowsAndLogsErrors_WhenCreationFails()
    {
        // Arrange
        var createUserDto = new CreateUserDto
        {
            User = new UserInfo() { Name = "newuser" },
            Password = "Password123!"
        };

        var identityErrors = new List<IdentityError>
        {
            new() { Description = "Password too weak" },
            new() { Description = "Username invalid" }
        };

        _userManager.Setup(x => x.FindByNameAsync(createUserDto.User.Name))
            .ReturnsAsync((User)null);

        _userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), createUserDto.Password))
            .ReturnsAsync(IdentityResult.Failed(identityErrors.ToArray()));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.CreateUserAsync(createUserDto, CancellationToken.None));

        Assert.Equal("Failed to create user", exception.Message);
        
        _roleService.Verify(
            x => x.GetRoleNamesByGuidAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _userManager.Verify(
            x => x.AddToRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateUserAsync_UpdatesUserSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var updateUserDto = new UpdateUserDto
        {
            User = new UpdateUserInfo() { Id = userId, Name = "updatedName" },
            Password = "newPassword",
            Roles = new List<Guid> { Guid.NewGuid() }
        };

        var user = new User { Id = userId };
        var roles = new List<string> { "Role1" };

        _userManager.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        _userManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string>());
        _roleService.Setup(x => x.GetRoleNamesByGuidAsync(updateUserDto.Roles, It.IsAny<CancellationToken>()))
            .ReturnsAsync(roles);
        _userManager.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);
        _userManager.Setup(x => x.RemovePasswordAsync(user)).ReturnsAsync(IdentityResult.Success);
        _userManager.Setup(x => x.AddPasswordAsync(user, updateUserDto.Password)).ReturnsAsync(IdentityResult.Success);

        // Act
        await _sut.UpdateUserAsync(updateUserDto, CancellationToken.None);

        // Assert
        _userManager.Verify(x => x.UpdateAsync(It.Is<User>(u => u.UserName == updateUserDto.User.Name)), Times.Once);
        _userManager.Verify(x => x.AddPasswordAsync(user, updateUserDto.Password), Times.Once);
    }

    [Fact]
    public async Task GetUserRolesByUserId_ThrowsException_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userManager.Setup(x => x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync((User)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _sut.GetUserRolesByUserId(userId, CancellationToken.None));
    }

    [Fact]
    public async Task CreateUserWithoutPasswordAsync_CreatesUserWithDefaultRole()
    {
        // Arrange
        var email = "test@test.com";
        var name = "testUser";
        var user = new User { Email = email, UserName = name };

        _userManager.Setup(x => x.CreateAsync(It.Is<User>(u => 
            u.Email == email && u.UserName == name)))
            .ReturnsAsync(IdentityResult.Success);
        _userManager.Setup(x => x.FindByNameAsync(name))
            .ReturnsAsync(user);
        _userManager.Setup(x => x.AddToRoleAsync(user, "User"))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await _sut.CreateUserWithoutPasswordAsync(email, name, CancellationToken.None);

        // Assert
        _userManager.Verify(x => x.CreateAsync(It.Is<User>(u => 
            u.Email == email && u.UserName == name)), Times.Once);
        _userManager.Verify(x => x.AddToRoleAsync(user, "User"), Times.Once);
    }

}