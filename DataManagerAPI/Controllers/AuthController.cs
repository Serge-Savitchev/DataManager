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
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="service"><see cref="IAuthService"/></param>
    /// <param name="logger"></param>
    public AuthController(IAuthService service, ILogger<AuthController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Register new user.
    /// </summary>
    /// <param name="user"><see cref="RegisteredUserDto"/></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>New user. <see cref="UserDto"/></returns>
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [HttpPost]
    [Route("register")]
    public async Task<ActionResult<UserDto>> RegisterUser([FromBody] RegisteredUserDto user,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started");

        var role = Enum.Parse<RoleIds>(user.Role, true);

        // only Admin can register new Admin or PowerUser.
        if (role == RoleIds.Admin || role == RoleIds.PowerUser)
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
            {
                _logger.LogWarning("Finished:{StatusCode}", StatusCodes.Status401Unauthorized);
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            if (Enum.Parse<RoleIds>(currentUser.User!.Role, true) != RoleIds.Admin)
            {
                _logger.LogWarning("Finished:{StatusCode}", StatusCodes.Status403Forbidden);
                return StatusCode(StatusCodes.Status403Forbidden);
            }
        }

        var result = await _service.RegisterUser(user, cancellationToken);

        _logger.LogDebug("{@result}", result);
        _logger.LogInformation("Finished:{StatusCode}", result.StatusCode);

        return StatusCode(result.StatusCode, result.Data);
    }

    /// <summary>
    /// Logins user.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>Pair of tokens. <see cref="LoginUserResponseDto"/></returns>
    [ProducesResponseType(typeof(LoginUserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<LoginUserResponseDto>> Login([FromBody] LoginUserDto user, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started");
        _logger.LogDebug("{login}", user.Login);

        var result = await _service.Login(user, cancellationToken);

        _logger.LogInformation("Finished:{StatusCode},UserId:{Id}", result.StatusCode, result.Data != null ? result.Data.Id : 0);

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
        _logger.LogInformation("Started");

        CurrentUserDto? currentUser = GetCurrentUser();
        if (currentUser == null)
        {
            _logger.LogWarning("Finished:{StatusCode}", StatusCodes.Status401Unauthorized);
            return StatusCode(StatusCodes.Status401Unauthorized);
        }

        _service.LogOut(currentUser.User!.Id);

        _logger.LogInformation("Finished:{StatusCode},UserId:{Id}", StatusCodes.Status200OK, currentUser.User!.Id);

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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Revoke(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started");

        CurrentUserDto? currentUser = GetCurrentUser();
        if (currentUser == null)
        {
            _logger.LogWarning("Finished:{StatusCode}", StatusCodes.Status401Unauthorized);
            return StatusCode(StatusCodes.Status401Unauthorized);
        }

        var result = await _service.Revoke(currentUser.User!.Id, cancellationToken);

        _logger.LogInformation("Finished:{StatusCode},UserId:{Id}", result.StatusCode, currentUser.User!.Id);

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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RefreshToken([FromBody] TokenApiModelDto tokens, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started");

        ResultWrapper<TokenApiModelDto> result = await _service.RefreshToken(tokens, cancellationToken);
        if (!result.Success)
        {
            _logger.LogWarning("Finished:{StatusCode}", result.StatusCode);
            return StatusCode(result.StatusCode, result.Message);
        }

        _logger.LogInformation("Finished:{StatusCode}", result.StatusCode);

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
        _logger.LogInformation("Started");

        ResultWrapper<int> result = await _service.UpdateUserPassword(userId, newPassword, cancellationToken);

        _logger.LogInformation("Finished:{StatusCode},UserId:{Id}", result.StatusCode, userId);

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
        _logger.LogInformation("Started");

        CurrentUserDto? currentUser = GetCurrentUser();
        if (currentUser == null)
        {
            _logger.LogWarning("Finished:{StatusCode}", StatusCodes.Status401Unauthorized);
            return StatusCode(StatusCodes.Status401Unauthorized);
        }

        var result = await _service.UpdateUserPassword(currentUser.User!.Id, newPassword, cancellationToken);

        _logger.LogInformation("Finished:{StatusCode},UserId:{Id}", result.StatusCode, currentUser.User!.Id);

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
    [Route("changerole/{userId}")]
    [Authorize(Policy = "PowerUser")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    public async Task<ActionResult<string>> UpdateUserRole(int userId,
        [FromBody][RoleValidation] string newRole, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started");

        if (userId == 1)
        {
            _logger.LogWarning("Finished:{StatusCode}", StatusCodes.Status403Forbidden);
            return StatusCode(StatusCodes.Status403Forbidden);  // can't change role for default admin
        }

        var result = await _service.UpdateUserRole(userId, newRole, cancellationToken);

        _logger.LogInformation("Finished:{StatusCode},UserId:{Id}", result.StatusCode, userId);

        return StatusCode(result.StatusCode, result.Data);
    }

    /// <summary>
    /// Returns user's detailed information.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns><see cref="UserDetailsDto"/></returns>
    [HttpGet]
    [Route("{userId}")]
    [Authorize(Policy = "Admin")]
    [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    public async Task<ActionResult<UserDetailsDto>> GetUserDetails(int userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started");

        var result = await _service.GetUserDetails(userId, cancellationToken);

        _logger.LogInformation("Finished:{StatusCode},UserId:{Id}", result.StatusCode, userId);

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
