using DataManagerAPI.Dto;
using DataManagerAPI.Helpers;
using DataManagerAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DataManagerAPI.Controllers;

[Route("api/users/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    [HttpPost]
    [Route("add")]
    public async Task<ActionResult<ResultWrapper<UserDto>>> AddUser([FromBody] AddUserDto user)
    {
        var result = await _service.AddUser(user);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut]
    [Route("update")]
    public async Task<ActionResult<ResultWrapper<UserDto>>> UpdateUser([FromBody] UserDto user)
    {
        var result = await _service.UpdateUser(user);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete]
    [Route("{userId}")]
    public async Task<ActionResult<ResultWrapper<UserDto>>> DeleteUser(int userId)
    {
        var result = await _service.DeleteUser(userId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet]
    [Route("{userId}")]
    public async Task<ActionResult<ResultWrapper<UserDto>>> GetUser(int userId)
    {
        var result = await _service.GetUser(userId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet]
    [Route("all")]
    public async Task<ActionResult<ResultWrapper<List<UserDto>>>> GetAllUsers()
    {
        var result = await _service.GetAllUsers();
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet]
    [Route("role/{roleName}")]
    public async Task<ActionResult<ResultWrapper<List<UserDto>>>> GetUsersByRole([RoleValidation] string roleName)
    {
        var result = await _service.GetUsersByRole(roleName);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut]
    [Route("credentials/{userId}")]
    public async Task<ActionResult<ResultWrapper<string>>> UpdateUserCredentials(int userId, [FromBody] UpdateCredentials updateCredentials)
    {
        var result = await _service.UpdateUserCredentials(userId, updateCredentials.Login, updateCredentials.Password);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet]
    [Route("credentials/{userId}")]
    public async Task<ActionResult<ResultWrapper<string>>> GetUserCredentials(int userId)
    {
        var result = await _service.GetUserCredentials(userId);
        return StatusCode(result.StatusCode, result);
    }

}
