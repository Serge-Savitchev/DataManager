using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DataManagerAPI.Controllers;

/// <summary>
/// Controller for users management.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUsersService _service;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="service"><see cref="IUsersService"/></param>
    public UsersController(IUsersService service)
    {
        _service = service;
    }

    /// <summary>
    /// Deletes user by Id.
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <returns>Deleted user. <see cref="UserDto"/></returns>
    [HttpDelete]
    [Authorize(Policy = "Admin")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserDto>> DeleteUser([FromQuery][Required] int userId)
    {
        var result = await _service.DeleteUser(userId);
        return StatusCode(result.StatusCode, result.Data);
    }

    /// <summary>
    /// Gets user by Id.
    /// </summary>
    /// <param name="UserId">User Id</param>
    /// <returns><see cref="UserDto"/></returns>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUser([FromQuery] int UserId)
    {
        var result = await _service.GetUser(UserId);
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
    public async Task<ActionResult<UserDto[]>> GetAllUsers()
    {
        var result = await _service.GetAllUsers();
        return StatusCode(result.StatusCode, result.Data);
    }

    /// <summary>
    /// Get users by role.
    /// </summary>
    /// <param name="role">Role name</param>
    /// <returns>Array of found users. <see cref="UserDto"/></returns>
    [HttpGet]
    [Route("role")]
    [Authorize(Policy = "Admin")]
    [ProducesResponseType(typeof(UserDto[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserDto[]>> GetUsersByRole([FromQuery][RoleValidation] string role)
    {
        var result = await _service.GetUsersByRole(role);
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
    public async Task<ActionResult<int>> UpdateOwners([FromBody] UpdateOwnerRequestDto request)
    {
        var result = await _service.UpdateOwners(request);
        return StatusCode(result.StatusCode, result.Data);
    }
}
