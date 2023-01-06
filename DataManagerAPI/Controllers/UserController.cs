using DataManagerAPI.Dto;
using DataManagerAPI.Helpers;
using DataManagerAPI.Services;
using Microsoft.AspNetCore.Authorization;
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

    [HttpDelete]
    [Route("{userId}")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<UserDto>> DeleteUser(int userId)
    {
        var result = await _service.DeleteUser(userId);
        return StatusCode(result.StatusCode, result.Data);
    }

    [HttpGet]
    [Route("{userId}")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetUser(int userId)
    {
        var result = await _service.GetUser(userId);
        return StatusCode(result.StatusCode, result.Data);
    }

    [HttpGet]
    [Route("all")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<List<UserDto>>> GetAllUsers()
    {
        var result = await _service.GetAllUsers();
        return StatusCode(result.StatusCode, result.Data);
    }

    [HttpGet]
    [Route("role/{roleName}")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<List<UserDto>>> GetUsersByRole([RoleValidation] string roleName)
    {
        var result = await _service.GetUsersByRole(roleName);
        return StatusCode(result.StatusCode, result.Data);
    }

    [HttpPut]
    [Route("updateowners")]
    public async Task<ActionResult<int>> UpdateOwners([FromBody] UpdateOwnerRequest request)
    {
        var result = await _service.UpdateOwners(request);
        return StatusCode(result.StatusCode, result.Data);
    }

    //private CurrentUserDataDto? GetCurrentUser()
    //{
    //    return HttpContext.Items["User"] as CurrentUserDataDto;
    //}
}
