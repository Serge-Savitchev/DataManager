using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Models;
using DataManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataManagerAPI.Controllers;

/// <summary>
/// Controller for managing user data.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserDataController : ControllerBase
{
    private readonly IUserDataService _service;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="service"><see cref="IUserDataService"/></param>
    public UserDataController(IUserDataService service)
    {
        _service = service;
    }

    /// <summary>
    /// Adds new user data.
    /// </summary>
    /// <param name="UserId">Id of user. If 0 then Id of current user is assumed.</param>
    /// <param name="data"><see cref="AddUserDataDto"/></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(UserDataDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [HttpPost]
    [Route("Add")]
    public async Task<ActionResult<UserDataDto>> AddUserData([FromBody] AddUserDataDto data, [FromQuery] int UserId = 0)
    {
        (int Code, int UserId) permission = CheckPermissions(UserId);
        if (permission.Code != StatusCodes.Status200OK)
        {
            return StatusCode(permission.Code);
        }

        ResultWrapper<UserDataDto> result = await _service.AddUserData(permission.UserId, 0, data);
        return StatusCode(result.StatusCode, result.Data);
    }

    /// <summary>
    /// Updates existing user data.
    /// </summary>
    /// <param name="data"><see cref="AddUserDataDto"/></param>
    /// <param name="UserDataId">Id of UserData</param>
    /// <param name="UserId">Id of user. If 0 then Id of current user is assumed.</param>
    /// <returns>Updated user data. <see cref="UserDataDto"/></returns>
    [ProducesResponseType(typeof(UserDataDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [HttpPut]
    [Route("Update")]
    public async Task<ActionResult<UserDataDto>> UpdateUserData([FromBody] AddUserDataDto data, [FromQuery] int UserDataId, [FromQuery] int UserId = 0)
    {
        (int Code, int UserId) permission = CheckPermissions(UserId);
        if (permission.Code != StatusCodes.Status200OK)
        {
            return StatusCode(permission.Code);
        }

        ResultWrapper<UserDataDto> result = await _service.UpdateUserData(permission.UserId, UserDataId, data);
        return StatusCode(result.StatusCode, result.Data);
    }

    /// <summary>
    /// Deletes user data by Id.
    /// </summary>
    /// <param name="UserDataId">Id of UserData</param>
    /// <param name="UserId">Id of user. If 0 then Id of current user is assumed.</param>
    /// <returns>Deleted user data. <see cref="UserDataDto"/></returns>
    [ProducesResponseType(typeof(UserDataDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [HttpDelete]
    [Route("Delete")]
    public async Task<ActionResult<UserDataDto>> DeleteUserData([FromQuery] int UserDataId, [FromQuery] int UserId = 0)
    {
        (int Code, int UserId) permission = CheckPermissions(UserId);
        if (permission.Code != StatusCodes.Status200OK)
        {
            return StatusCode(permission.Code);
        }

        ResultWrapper<UserDataDto> result = await _service.DeleteUserData(permission.UserId, UserDataId);
        return StatusCode(result.StatusCode, result.Data);
    }

    /// <summary>
    /// Gets user data by Id.
    /// </summary>
    /// <param name="UserDataId">Id of UserData</param>
    /// <param name="UserId">Id of user. If 0 then Id of current user is assumed.</param>
    /// <returns>User data. <see cref="UserDataDto"/></returns>
    [ProducesResponseType(typeof(UserDataDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    [HttpGet]
    public async Task<ActionResult<UserDataDto>> GetUserData([FromQuery] int UserDataId, [FromQuery] int UserId = 0)
    {
        (int Code, int UserId) permission = CheckPermissions(UserId);
        if (permission.Code != StatusCodes.Status200OK)
        {
            return StatusCode(permission.Code);
        }

        ResultWrapper<UserDataDto> result = await _service.GetUserData(permission.UserId, UserDataId);
        return StatusCode(result.StatusCode, result.Data);
    }

    /// <summary>
    /// Gets array of user data by user Id.
    /// </summary>
    /// <param name="UserId">Id of user. If 0 then Id of current user is assumed.</param>
    /// <returns>Array of user's data. <see cref="UserDataDto"/></returns>
    [ProducesResponseType(typeof(UserDataDto[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet]
    [Route("All")]
    public async Task<ActionResult<UserDataDto[]>> GetUserDataByUserId([FromQuery] int UserId = 0)
    {
        (int Code, int UserId) permission = CheckPermissions(UserId);
        if (permission.Code != StatusCodes.Status200OK)
        {
            return StatusCode(permission.Code);
        }

        ResultWrapper<UserDataDto[]> result = await _service.GetUserDataByUserId(permission.UserId);
        return StatusCode(result.StatusCode, result.Data);
    }

    /// <summary>
    /// Gets current user from HttpContext.
    /// </summary>
    /// <returns><see cref="CurrentUserDto"/></returns>
    private CurrentUserDto? GetCurrentUser()
    {
        return HttpContext.Items["User"] as CurrentUserDto;
    }

    private (int Code, int UserId) CheckPermissions(int userId)
    {
        CurrentUserDto? currentUser = GetCurrentUser();
        if (currentUser == null)
        {
            return (Code: StatusCodes.Status401Unauthorized, UserId: userId);
        }

        if (userId <= 0)    // if not set then set Id of current user
        {
            return (Code: StatusCodes.Status200OK, UserId: currentUser.User!.Id);
        }

        if (currentUser.User!.Id != userId &&
            Enum.Parse<RoleIds>(currentUser.User.Role, true) != RoleIds.Admin)
        {
            return (Code: StatusCodes.Status403Forbidden, UserId: currentUser.User!.Id);
        }

        return (Code: StatusCodes.Status200OK, UserId: userId);
    }
}
