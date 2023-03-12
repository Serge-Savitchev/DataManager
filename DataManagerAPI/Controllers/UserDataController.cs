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
    private readonly ILogger<UserDataController> _logger;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="service"><see cref="IUserDataService"/></param>
    /// <param name="logger"></param>
    public UserDataController(IUserDataService service, ILogger<UserDataController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Adds new user data.
    /// </summary>
    /// <param name="userId">Id of user. If 0 then Id of current user is assumed.</param>
    /// <param name="data"><see cref="AddUserDataDto"/></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(UserDataDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [HttpPost]
    [Route("{userId}")]
    public async Task<ActionResult<UserDataDto>> AddUserData([FromRoute] int userId, [FromBody] AddUserDataDto data)
    {
        _logger.LogInformation("Started");

        (int Code, int UserId) permission = CheckPermissions(userId);
        if (permission.Code != StatusCodes.Status200OK)
        {
            _logger.LogInformation("Finished:{StatusCode},UserId:{Id}", permission.Code, permission.UserId);
            return StatusCode(permission.Code);
        }

        ResultWrapper<UserDataDto> result = await _service.AddUserData(permission.UserId, data);

        _logger.LogInformation("Finished");

        return StatusCode(result.StatusCode, result.Data);
    }

    /// <summary>
    /// Updates existing user data.
    /// </summary>
    /// <param name="userId">Id of user. If 0 then Id of current user is assumed.</param>
    /// <param name="userDataId">Id of UserData</param>
    /// <param name="data"><see cref="AddUserDataDto"/></param>
    /// <returns>Updated user data. <see cref="UserDataDto"/></returns>
    [ProducesResponseType(typeof(UserDataDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut]
    [Route("{userId}/{userDataId}")]
    public async Task<ActionResult<UserDataDto>> UpdateUserData([FromRoute] int userId, [FromRoute] int userDataId,
        [FromBody] AddUserDataDto data)
    {
        _logger.LogInformation("Started");

        (int Code, int UserId) permission = CheckPermissions(userId);
        if (permission.Code != StatusCodes.Status200OK)
        {
            _logger.LogInformation("Finished:{StatusCode},{Id},{userDataId}", permission.Code, permission.UserId, userDataId);

            return StatusCode(permission.Code);
        }

        ResultWrapper<UserDataDto> result = await _service.UpdateUserData(permission.UserId, userDataId, data);

        _logger.LogInformation("Finished");

        return StatusCode(result.StatusCode, result.Data);
    }

    /// <summary>
    /// Deletes user data by Id.
    /// </summary>
    /// <param name="userId">Id of user. If 0 then Id of current user is assumed.</param>
    /// <param name="userDataId">Id of UserData</param>
    /// <returns>Deleted user data. <see cref="UserDataDto"/></returns>
    [ProducesResponseType(typeof(UserDataDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete]
    [Route("{userId}/{userDataId}")]

    public async Task<ActionResult<UserDataDto>> DeleteUserData([FromRoute] int userId, [FromRoute] int userDataId)
    {
        _logger.LogInformation("Started");

        (int Code, int UserId) permission = CheckPermissions(userId);
        if (permission.Code != StatusCodes.Status200OK)
        {
            _logger.LogInformation("Finished:{StatusCode},{Id},{userDataId}", permission.Code, permission.UserId, userDataId);

            return StatusCode(permission.Code);
        }

        ResultWrapper<UserDataDto> result = await _service.DeleteUserData(permission.UserId, userDataId);

        _logger.LogInformation("Finished");

        return StatusCode(result.StatusCode, result.Data);
    }

    /// <summary>
    /// Gets user data by Id.
    /// </summary>
    /// <param name="userId">Id of user. If 0 then Id of current user is assumed.</param>
    /// <param name="userDataId">Id of UserData</param>
    /// <returns>User data. <see cref="UserDataDto"/></returns>
    [ProducesResponseType(typeof(UserDataDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    [HttpGet]
    [Route("{userId}/{userDataId}")]
    public async Task<ActionResult<UserDataDto>> GetUserData([FromRoute] int userId, [FromRoute] int userDataId)
    {
        _logger.LogInformation("Started");

        (int Code, int UserId) permission = CheckPermissions(userId);
        if (permission.Code != StatusCodes.Status200OK)
        {
            _logger.LogInformation("Finished:{StatusCode},{Id},{userDataId}", permission.Code, permission.UserId, userDataId);

            return StatusCode(permission.Code);
        }

        ResultWrapper<UserDataDto> result = await _service.GetUserData(permission.UserId, userDataId);

        _logger.LogInformation("Finished");

        return StatusCode(result.StatusCode, result.Data);
    }

    /// <summary>
    /// Gets array of user data by user Id.
    /// </summary>
    /// <param name="userId">Id of user. If 0 then Id of current user is assumed.</param>
    /// <returns>Array of user's data. <see cref="UserDataDto"/></returns>
    [ProducesResponseType(typeof(UserDataDto[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet]
    [Route("{userId}/All")]
    public async Task<ActionResult<UserDataDto[]>> GetUserDataByUserId([FromRoute] int userId)
    {
        (int Code, int UserId) permission = CheckPermissions(userId);
        if (permission.Code != StatusCodes.Status200OK)
        {
            _logger.LogInformation("Finished:{StatusCode},{Id}", permission.Code, permission.UserId);
            return StatusCode(permission.Code);
        }

        ResultWrapper<UserDataDto[]> result = await _service.GetUserDataByUserId(permission.UserId);

        _logger.LogInformation("Finished");

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

        if (userId <= 0)    // if not set then set Id of current user
        {
            return (Code: StatusCodes.Status200OK, UserId: currentUser!.User!.Id);
        }

        if (currentUser!.User!.Id != userId &&
            Enum.Parse<RoleIds>(currentUser.User.Role, true) != RoleIds.Admin)
        {
            return (Code: StatusCodes.Status403Forbidden, UserId: currentUser.User!.Id);
        }

        return (Code: StatusCodes.Status200OK, UserId: userId);
    }
}
