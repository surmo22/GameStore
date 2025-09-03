using GameStore.BLL.DTOs.Comment;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.Common.Constants;
using GameStore.Domain.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.WebApi.Controllers;

/// <summary>
/// The CommentsController provides API endpoints for managing comments-related functionality
/// such as retrieving ban durations and banning users.
/// </summary>
[Route("comments/ban")]
[Authorize(Policy = nameof(UserPermissionTypes.BanUsers))]
public class BanController(IUserBanService userBanService) : ControllerBase
{
    /// Retrieves the available ban durations for users in a readable format.
    /// This method queries all possible `BanDuration` enumeration values, converts them
    /// into a human-readable description, and returns the results as a list of strings.
    /// <returns>A list of strings representing the available ban durations.</returns>
    [HttpGet("durations")]
    public IActionResult GetBanDurations()
    {
        var durations = Enum.GetValues(typeof(BanDuration))
            .Cast<BanDuration>()
            .ToList();
        return Ok(durations);
    }

    /// Bans a user for the specified duration based on the provided request details.
    /// <param name="request">
    /// The request containing the username and the ban duration details.
    /// </param>
    /// <param name="token">
    /// The cancellation token used to propagate notification of cancellation operations.
    /// </param>
    /// <returns>
    /// An IActionResult indicating the outcome of the ban operation.
    /// </returns>
    [HttpPost("")]
    public async Task<IActionResult> BanUser([FromBody] BanUserRequest request, CancellationToken token)
    {
        await userBanService.BanUserAsync(request.UserName, request, token);
        return Ok();
    }
}