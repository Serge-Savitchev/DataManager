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
}
