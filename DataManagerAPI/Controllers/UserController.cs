using DataManagerAPI.Dto;
using DataManagerAPI.Helpers;
using DataManagerAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DataManagerAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    [HttpPost]
    [Route("Add")]
    public async Task<ActionResult<ResultWrapper<UserDto>>> AddUser([FromBody] AddUserDto user)
    {
        var result = await _service.AddUser(user);
        return StatusCode(result.StatusCode, result);
    }
}
