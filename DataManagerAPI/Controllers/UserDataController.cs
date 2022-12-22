using DataManagerAPI.Dto;
using DataManagerAPI.Helpers;
using DataManagerAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DataManagerAPI.Controllers;

[Route("api/userdata/[controller]")]
[ApiController]
public class UserDataController : ControllerBase
{
    private readonly IUserDataService _service;

    public UserDataController(IUserDataService service)
    {
        _service = service;
    }

    [HttpPost]
    [Route("Add")]
    public async Task<ActionResult<ResultWrapper<UserDataDto>>> AddUserData([FromBody] AddUserDataDto data)
    {
        var result = await _service.AddUserData(data);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut]
    [Route("Update")]
    public async Task<ActionResult<ResultWrapper<UserDataDto>>> UpdateUserData([FromBody] UserDataDto data)
    {
        var result = await _service.UpdateUserData(data);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete]
    [Route("{UserId}")]
    public async Task<ActionResult<ResultWrapper<UserDataDto>>> UpdateUserData(int UserId)
    {
        var result = await _service.DeleteUserData(UserId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet]
    [Route("{userDataId}")]
    public async Task<ActionResult<ResultWrapper<UserDto>>> GetUser(int userDataId)
    {
        var result = await _service.GetUserData(userDataId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet]
    [Route("all/{userId}")]
    public async Task<ActionResult<ResultWrapper<List<UserDto>>>> GetUserDataByUserId(int userId)
    {
        var result = await _service.GetUserDataByUserId(userId);
        return StatusCode(result.StatusCode, result);
    }
}
