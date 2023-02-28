using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Models;
using DataManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DataManagerAPI.Controllers;

/// <summary>
/// Authentication/authorization controller
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="service"><see cref="IAuthService"/></param>
    public AuthController(IAuthService service)
    {
        _service = service;
    }

    /// <summary>
    /// Register new user.
    /// </summary>
    /// <param name="user"><see cref="RegisterUserDto"/></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>New user. <see cref="UserDto"/></returns>
    [HttpPost]
    [Route("register")]
    public async Task<ActionResult<UserDto>> RegisterUser([FromBody] RegisterUserDto user,
        CancellationToken cancellationToken = default)
    {
        var role = Enum.Parse<RoleIds>(user.Role, true);

        // only Admin can register new Admin or PowerUser.
        if (role == RoleIds.Admin || role == RoleIds.PowerUser)
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            if (Enum.Parse<RoleIds>(currentUser.User!.Role, true) != RoleIds.Admin)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }
        }

        var result = await _service.RegisterUser(user, cancellationToken);
        return StatusCode(result.StatusCode, result.Data);
    }

    /// <summary>
    /// Logins user.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>Pair of tokens. <see cref="LoginUserResponseDto"/></returns>
    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<LoginUserResponseDto>> Login([FromBody] LoginUserDto user, CancellationToken cancellationToken = default)
    {
        var result = await _service.Login(user, cancellationToken);
        return StatusCode(result.StatusCode, result.Data);
    }

    /// <summary>
    /// Logout.
    /// </summary>
    /// <returns>User Id</returns>
    [HttpPost]
    [Route("logout")]
    [Authorize]
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
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
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns></returns>
    [HttpPost]
    [Route("revoke")]
    [Authorize]
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Revoke(CancellationToken cancellationToken = default)
    {
        CurrentUserDto? currentUser = GetCurrentUser();
        if (currentUser == null)
        {
            return StatusCode(StatusCodes.Status401Unauthorized);
        }

        var result = await _service.Revoke(currentUser.User!.Id, cancellationToken);
        return StatusCode(result.StatusCode);
    }

    /// <summary>
    /// Refresh pair of tokens.
    /// </summary>
    /// <param name="tokens"><see cref="TokenApiModelDto"/></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>New pair of tokens. <see cref="TokenApiModelDto"/></returns>
    [HttpPost]
    [Route("refresh")]
    [ProducesResponseType(typeof(TokenApiModelDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] TokenApiModelDto tokens, CancellationToken cancellationToken = default)
    {
        ResultWrapper<TokenApiModelDto> result = await _service.RefreshToken(tokens, cancellationToken);
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
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns></returns>
    [HttpPut]
    [Route("changepassword/{userId}")]
    [Authorize(Policy = "Admin")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    public async Task<IActionResult> UpdateUserPassword(int userId, [FromBody][Required] string newPassword
        , CancellationToken cancellationToken = default)
    {
        ResultWrapper<int> result = await _service.UpdateUserPassword(userId, newPassword, cancellationToken);
        return StatusCode(result.StatusCode);
    }

    /// <summary>
    /// Sets new user's password. Allowed to current user only.
    /// </summary>
    /// <param name="newPassword"></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns></returns>
    [HttpPut]
    [Route("changepassword")]
    [Authorize]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdatePassword([FromBody][Required] string newPassword, CancellationToken cancellationToken = default)
    {
        CurrentUserDto? currentUser = GetCurrentUser();
        if (currentUser == null)
        {
            return StatusCode(StatusCodes.Status401Unauthorized);
        }

        var result = await _service.UpdateUserPassword(currentUser.User!.Id, newPassword, cancellationToken);
        return StatusCode(result.StatusCode);
    }

    /// <summary>
    /// Changes user's role.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="newRole"></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>New role</returns>
    [HttpPut]
    [Route("changerole")]
    [Authorize(Policy = "PowerUser")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    public async Task<ActionResult<string>> UpdateUserRole([FromQuery][Required] int userId,
        [FromBody][RoleValidation] string newRole, CancellationToken cancellationToken = default)
    {
        if (userId == 1)
        {
            return StatusCode(StatusCodes.Status403Forbidden);  // can't change role for default admin
        }

        var result = await _service.UpdateUserRole(userId, newRole, cancellationToken);
        return StatusCode(result.StatusCode, result.Data);
    }

    /// <summary>
    /// Returns user's detailed information.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns><see cref="UserDetailsDto"/></returns>
    [HttpGet]
    [Authorize(Policy = "Admin")]
    [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    public async Task<ActionResult<UserDetailsDto>> GetUserDetails([FromQuery][Required] int userId, CancellationToken cancellationToken = default)
    {
        var result = await _service.GetUserDetails(userId, cancellationToken);
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
}
