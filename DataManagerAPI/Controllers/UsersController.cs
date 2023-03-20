using DataManagerAPI.Dto;
using DataManagerAPI.Dto.Helpers;
using DataManagerAPI.Dto.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataManagerAPI.Controllers;

/// <summary>
/// Controller for users management.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUsersService _service;
    private readonly ILogger<UsersController> _logger;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="service"><see cref="IUsersService"/></param>
    /// <param name="logger"></param>
    public UsersController(IUsersService service, ILogger<UsersController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Deletes user by Id.
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <returns>Deleted user. <see cref="UserDto"/></returns>
    [HttpDelete]
    [Route("{userId}")]
    [Authorize(Policy = "Admin")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> DeleteUser(int userId)
    {
        _logger.LogInformation("Started");

        var result = await _service.DeleteUser(userId);

        _logger.LogInformation("Finished");

        return StatusCode(result.StatusCode, result.Data);
    }

    /// <summary>
    /// Gets user by Id.
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <returns><see cref="UserDto"/></returns>
    [HttpGet]
    [Route("{userId}")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUser(int userId)
    {
        _logger.LogInformation("Started");

        var result = await _service.GetUser(userId);

        _logger.LogInformation("Finished");

        return StatusCode(result.StatusCode, result.Data);
    }

    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <returns>Array of all users. <see cref="UserDto"/></returns>
    [HttpGet]
    [Route("all")]
    [Authorize(Policy = "Admin")]
    [ProducesResponseType(typeof(UserDto[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UserDto[]>> GetAllUsers()
    {
        _logger.LogInformation("Started");

        var result = await _service.GetAllUsers();

        _logger.LogInformation("Finished");

        return StatusCode(result.StatusCode, result.Data);
    }

    /// <summary>
    /// Get users by role.
    /// </summary>
    /// <param name="role">Role name</param>
    /// <returns>Array of found users. <see cref="UserDto"/></returns>
    [HttpGet]
    [Route("role/{role}")]
    [Authorize(Policy = "Admin")]
    [ProducesResponseType(typeof(UserDto[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UserDto[]>> GetUsersByRole([RoleValidation] string role)
    {
        _logger.LogInformation("Started");

        var result = await _service.GetUsersByRole(role);

        _logger.LogInformation("Finished");

        return StatusCode(result.StatusCode, result.Data);
    }

    /// <summary>
    /// Update owner of users.
    /// </summary>
    /// <param name="request"><see cref="UpdateOwnerRequestDto"/></param>
    /// <returns>Count of users with changed owner</returns>
    [HttpPut]
    [Route("updateowners")]
    [Authorize(Policy = "PowerUser")]
    [ProducesResponseType(typeof(int[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<int>> UpdateOwners([FromBody] UpdateOwnerRequestDto request)
    {
        _logger.LogInformation("Started");

        var result = await _service.UpdateOwners(request);

        _logger.LogInformation("Finished");

        return StatusCode(result.StatusCode, result.Data);
    }
}
