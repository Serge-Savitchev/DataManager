using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DataManagerAPI.Controllers
{
    /// <summary>
    /// Authentication/authorization controllr
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="service"></param>
        public AuthController(IAuthService service)
        {
            _service = service;
        }

        /// <summary>
        /// Register new user.
        /// </summary>
        /// <param name="user"><see cref="RegisterUserDto"/>.</param>
        /// <returns>New user. <see cref="UserDto"/>.</returns>
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<UserDto>> RegisterUser([FromBody] RegisterUserDto user)
        {
            var result = await _service.RegisterUser(user);
            return StatusCode(result.StatusCode, result.Data);
        }

        /// <summary>
        /// Logins user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Pair of tokens. <see cref="LoginUserResponseDto"/>.</returns>
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<LoginUserResponseDto>> Login([FromBody] LoginUserDto user)
        {
            var result = await _service.Login(user);
            return StatusCode(result.StatusCode, result.Data);
        }

        /// <summary>
        /// Logout.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult LogOut()
        {
            CurrentUserDto? currentUser = GetCurrentUser();
            if (currentUser == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            _service.LogOut(currentUser.User!.Id);
            return Ok();
        }

        /// <summary>
        /// Revokes refresh token.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("revoke")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        /// <summary>
        /// Refresh pair of tokens.
        /// </summary>
        /// <param name="tokens"><see cref="TokenApiModelDto"/>.</param>
        /// <returns>New pair of tokens. <see cref="TokenApiModelDto"/>.</returns>
        [HttpPost]
        [Route("refresh")]
        [ProducesResponseType(typeof(TokenApiModelDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] TokenApiModelDto tokens)
        {
            ResultWrapper<TokenApiModelDto> result = await _service.RefreshToken(tokens);
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Message);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Sets new user's password. Allowed to Admin only.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPassword"></param>
        /// <returns>User Id.</returns>
        [HttpPut]
        [Route("changepassword/{userId}")]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        public async Task<IActionResult> UpdateUserPassword(int userId, [FromBody][Required] string newPassword)
        {
            ResultWrapper<int> result = await _service.UpdateUserPassword(userId, newPassword);
            return StatusCode(result.StatusCode);
        }

        /// <summary>
        /// Sets new user's password. Allowed to current user only.
        /// </summary>
        /// <param name="newPassword"></param>
        /// <returns>User Id.</returns>
        [HttpPut]
        [Route("changepassword")]
        [Authorize]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdatePassword([FromBody][Required] string newPassword)
        {
            CurrentUserDto? currentUser = GetCurrentUser();
            if (currentUser == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            var result = await _service.UpdateUserPassword(currentUser.User!.Id, newPassword);
            return StatusCode(result.StatusCode);
        }

        /// <summary>
        /// Changes user's role.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newRole"></param>
        /// <returns>New role.</returns>
        [HttpPut]
        [Route("changerole/{userId}")]
        [Authorize(Policy = "PowerUser")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        public async Task<ActionResult<string>> UpdateUserRole(int userId, [FromBody][RoleValidation] string newRole)
        {
            var result = await _service.UpdateUserRole(userId, newRole);
            return StatusCode(result.StatusCode, result.Data);
        }

        /// <summary>
        /// Returns user's detailed information.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns><see cref="UserDetailsDto"/>.</returns>
        [HttpGet]
        [Route("{userId}")]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        public async Task<ActionResult<UserDetailsDto>> GetUserDetails(int userId)
        {
            var result = await _service.GetUserDetails(userId);
            return StatusCode(result.StatusCode, result.Data);
        }

        private CurrentUserDto? GetCurrentUser()
        {
            return HttpContext.Items["User"] as CurrentUserDto;
        }
    }
}
