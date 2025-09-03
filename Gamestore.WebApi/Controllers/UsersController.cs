using GameStore.BLL.DTOs.Notifications;
using GameStore.BLL.DTOs.Role;
using GameStore.BLL.DTOs.User;
using GameStore.BLL.DTOs.User.Creation;
using GameStore.BLL.DTOs.User.Login;
using GameStore.BLL.DTOs.User.Update;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.BLL.Interfaces.NotificationServices;
using GameStore.BLL.Interfaces.Security;
using GameStore.Domain.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.WebApi.Controllers;

/// <summary>
/// Controller responsible for managing user-related operations.
/// </summary>
/// <remarks>
/// Handles user creation, retrieval, updating, deletion, authentication, and role management.
/// Secured by policies requiring appropriate permissions as defined in UserPermissionTypes.
/// </remarks>
[ApiController]
[Route("[controller]")]
public class UsersController(
    IUserService           userService,
    IAuthenticationService authenticationService,
    IPageAccessChecker     pageAccessChecker,
    INotificationService   notificationService) : ControllerBase
{
    /// <summary>
    /// Checks if the current user has permission to access the specified page and target.
    /// </summary>
    /// <param name="request">Object containing the target page and optional target ID.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A boolean indicating whether access is granted.</returns>
    [HttpPost("access")]
    public async Task<ActionResult<bool>> GetAccess([FromBody] CheckAccessRequestDto request, CancellationToken cancellationToken)
    {
        var hasAccess = await pageAccessChecker.CheckAccessAsync(request, cancellationToken);
        return Ok(hasAccess);
    }

    /// <summary>
    /// Authenticates and logs in a user based on the provided login request details.
    /// </summary>
    /// <param name="loginRequestDto">An object containing the user's login information, such as credentials.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="LoginResponseDto"/> containing the authentication token if login is successful.</returns>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto loginRequestDto, CancellationToken cancellationToken)
    {
        var response = await authenticationService.LoginAsync(loginRequestDto, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Retrieves a list of all users in the system.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A list of <see cref="UserDto"/> objects representing all users.</returns>
    [HttpGet]
    [Authorize(Policy = nameof(UserPermissionTypes.ManageUsers))]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var users = await userService.GetAllUsers(cancellationToken);
        return Ok(users);
    }

    /// <summary>
    /// Retrieves a user's details based on their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="UserDto"/> containing the user's details.</returns>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = nameof(UserPermissionTypes.ManageUsers))]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var user = await userService.GetUserById(id, cancellationToken);
        return Ok(user);
    }

    /// <summary>
    /// Deletes a user identified by the specified unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to be deleted.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = nameof(UserPermissionTypes.ManageUsers))]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        await userService.DeleteUserById(id, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Creates a new user with the specified details and assigns roles to the user.
    /// </summary>
    /// <param name="createUserDto">An object containing the details required for creating the user, including user information, roles, and password.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the outcome of the operation.</returns>
    [HttpPost]
    [Authorize(Policy = nameof(UserPermissionTypes.ManageUsers))]
    public async Task<IActionResult> CreateUser(CreateUserDto createUserDto, CancellationToken cancellationToken)
    {
        await userService.CreateUserAsync(createUserDto, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Updates the details of an existing user, including their roles and password.
    /// </summary>
    /// <param name="updateUserDto">An object containing the user's updated information, such as user details, roles, and password.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [HttpPut]
    [Authorize(Policy = nameof(UserPermissionTypes.ManageUsers))]
    public async Task<IActionResult> UpdateUser(UpdateUserDto updateUserDto, CancellationToken cancellationToken)
    {
        await userService.UpdateUserAsync(updateUserDto, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Retrieves the roles assigned to a user based on their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user whose roles are to be retrieved.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A list of <see cref="RoleDto"/> objects representing the roles assigned to the user.</returns>
    [HttpGet("{id:guid}/roles")]
    [Authorize(Policy = nameof(UserPermissionTypes.ManageUsers))]
    public async Task<IActionResult> GetUserRoles(Guid id, CancellationToken cancellationToken)
    {
        var roles = await userService.GetUserRolesByUserId(id, cancellationToken);
        return Ok(roles);
    }

    [HttpGet("notifications")]
    [Authorize]
    public IActionResult GetSupportedNotificationTypes()
    {
        var options = Enum.GetValues(typeof(UserNotificationTypes))
            .Cast<UserNotificationTypes>()
            .ToList();
        
        return Ok(options);
    }

    [HttpGet("my/notifications")]
    [Authorize]
    public async Task<IActionResult> GetMyNotifications(CancellationToken cancellationToken)
    {
        var options = await notificationService.GetUserNotificationTypes(cancellationToken);
        return Ok(options);
    }

    [HttpPut("notifications")]
    [Authorize]
    public async Task<IActionResult> UpdateUserNotifications(
        NotificationsList notificationsList,
        CancellationToken cancellationToken)
    {
        await notificationService.UpdateUserNotificationTypes(notificationsList.Notifications, cancellationToken);
        return Ok();
    }
}