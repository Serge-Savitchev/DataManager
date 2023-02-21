using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Services;
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
    /// <param name="data"><see cref="AddUserDataDto"/></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(UserDataDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost]
    [Route("Add")]
    public async Task<ActionResult<UserDataDto>> AddUserData([FromBody] AddUserDataDto data)
    {
        ResultWrapper<UserDataDto> result = await _service.AddUserData(data);
        return StatusCode(result.StatusCode, result.Data);
    }

    /// <summary>
    /// Updates existing user data.
    /// </summary>
    /// <param name="data"><see cref="UserDataDto"/></param>
    /// <returns>Updated user data. <see cref="UserDataDto"/></returns>
    [ProducesResponseType(typeof(UserDataDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut]
    [Route("Update")]
    public async Task<ActionResult<UserDataDto>> UpdateUserData([FromBody] UserDataDto data)
    {
        ResultWrapper<UserDataDto> result = await _service.UpdateUserData(data);
        return StatusCode(result.StatusCode, result.Data);
    }

    /// <summary>
    /// Deletes user data by Id.
    /// </summary>
    /// <param name="UserDataId"></param>
    /// <returns>Deleted user data. <see cref="UserDataDto"/></returns>
    [HttpDelete]
    [Route("{UserId}")]
    public async Task<ActionResult<UserDataDto>> DeleteUserData(int UserDataId)
    {
        ResultWrapper<UserDataDto> result = await _service.DeleteUserData(UserDataId);
        return StatusCode(result.StatusCode, result.Data);
    }

    /// <summary>
    /// Gets user data by Id.
    /// </summary>
    /// <param name="userDataId">Id of UserData</param>
    /// <returns>User data. <see cref="UserDataDto"/></returns>
    [ProducesResponseType(typeof(UserDataDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet]
    [Route("{userDataId}")]
    public async Task<ActionResult<UserDataDto>> GetUserData(int userDataId)
    {
        ResultWrapper<UserDataDto> result = await _service.GetUserData(userDataId);
        return StatusCode(result.StatusCode, result.Data);
    }

    /// <summary>
    /// Gets array of user data by user Id.
    /// </summary>
    /// <param name="userId">Id of user</param>
    /// <returns>Array of user' data. <see cref="UserDataDto"/></returns>
    [ProducesResponseType(typeof(UserDataDto[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet]
    [Route("all/{userId}")]
    public async Task<ActionResult<UserDataDto[]>> GetUserDataByUserId(int userId)
    {
        ResultWrapper<UserDataDto[]> result = await _service.GetUserDataByUserId(userId);
        return StatusCode(result.StatusCode, result.Data);
    }
}
