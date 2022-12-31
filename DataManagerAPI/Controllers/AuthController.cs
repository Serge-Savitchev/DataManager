using DataManagerAPI.Dto;
using DataManagerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DataManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<UserDto>> RegisterUser([FromBody] RegisterUserDto user)
        {
            var result = await _service.RegisterUser(user);
            return StatusCode(result.StatusCode, result.Data);
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<LoginUserResponseDto>> Login([FromBody] LoginUserDto user)
        {
            var result = await _service.Login(user);
            return StatusCode(result.StatusCode, result.Data);
        }

        [HttpPost]
        [Route("revoke")]
        public async Task<IActionResult> Revoke()
        {
            CurrentUserDto? currentUser = GetCurrentUser();
            if (currentUser == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            var result = await _service.Revoke(currentUser.User!.Id);
            return StatusCode(result.StatusCode);
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenApiModelDto tokens)
        {
            var result = await _service.RefreshToken(tokens);
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Message);
            }

            return Ok(result.Data);
        }

        [HttpPut]
        [Route("credentials/{userId}")]
        [Authorize]
        public async Task<IActionResult> UpdateUserPassword(int userId, [FromBody][Required] string newPassword)
        {
            var result = await _service.UpdateUserPassword(userId, newPassword);
            return StatusCode(result.StatusCode);
        }

        private CurrentUserDto? GetCurrentUser()
        {
            return HttpContext.Items["User"] as CurrentUserDto;
        }
    }
}
