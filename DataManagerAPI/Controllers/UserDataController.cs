using DataManagerAPI.Dto;
using DataManagerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataManagerAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserDataController : ControllerBase
{
    private readonly IUserDataService _service;

    public UserDataController(IUserDataService service)
    {
        _service = service;
    }

    [HttpPost]
    [Route("Add")]
    public async Task<ActionResult<UserDataDto>> AddUserData([FromBody] AddUserDataDto data)
    {
        var result = await _service.AddUserData(data);
        return StatusCode(result.StatusCode, result.Data);
    }

    [HttpPut]
    [Route("Update")]
    public async Task<ActionResult<UserDataDto>> UpdateUserData([FromBody] UserDataDto data)
    {
        var result = await _service.UpdateUserData(data);
        return StatusCode(result.StatusCode, result.Data);
    }

    [HttpDelete]
    [Route("{UserId}")]
    public async Task<ActionResult<UserDataDto>> UpdateUserData(int UserId)
    {
        var result = await _service.DeleteUserData(UserId);
        return StatusCode(result.StatusCode, result.Data);
    }

    [HttpGet]
    [Route("{userDataId}")]
    public async Task<ActionResult<UserDto>> GetUser(int userDataId)
    {
        var result = await _service.GetUserData(userDataId);
        return StatusCode(result.StatusCode, result.Data);
    }

    [HttpGet]
    [Route("all/{userId}")]
    public async Task<ActionResult<List<UserDto>>> GetUserDataByUserId(int userId)
    {
        var result = await _service.GetUserDataByUserId(userId);
        return StatusCode(result.StatusCode, result.Data);
    }
}
