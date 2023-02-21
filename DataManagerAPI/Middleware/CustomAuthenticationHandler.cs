using DataManagerAPI.Dto;
using DataManagerAPI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace DataManagerAPI.Middleware;

/// <summary>
/// Custom authentication handler.
/// </summary>
public class CustomAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ITokenService _tokenService;
    private readonly ILoggedOutUsersCollectionService _loggedOutUsersCollectionService;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="tokenService"><see cref="ITokenService"/></param>
    /// <param name="options"><see cref="AuthenticationSchemeOptions"/></param>
    /// <param name="logger"><see cref="ILoggerFactory"/></param>
    /// <param name="encoder"><see cref="UrlEncoder"/></param>
    /// <param name="clock"><see cref="ISystemClock"/></param>
    /// <param name="loggedOutUsersCollectionService"><see cref="ILoggedOutUsersCollectionService"/></param>
    public CustomAuthenticationHandler(
        ITokenService tokenService,
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        ILoggedOutUsersCollectionService loggedOutUsersCollectionService) : base(options, logger, encoder, clock)
    {
        _tokenService = tokenService;
        _loggedOutUsersCollectionService = loggedOutUsersCollectionService;
    }

    /// <summary>
    /// Authenticate handler.
    /// </summary>
    /// <returns><see cref="AuthenticateResult"/></returns>
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Take token from request header.
        // Assume that "Authorization" header has the following format: "Bearer <token value>".
        var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        // Validate token. Token life time is checked.
        ClaimsPrincipal? validationData = _tokenService.ValidateToken(token!, useLifetime: true);

        if (validationData != null) // validation passed
        {
            // Create current user data from claims.
            CurrentUserDto user = _tokenService.CreateCurrentUser(validationData.Claims);

            // Check if user logged out.
            if (_loggedOutUsersCollectionService.Contains(user.User!.Id))
            {
                // if yes, return AuthenticateResult.Fail
                return Task.FromResult(AuthenticateResult.Fail("Unauthorized"));
            }

            // attach user to context
            Context.Items["User"] = user;
            Context.User = validationData;

            // create ticket
            var authTicket = new AuthenticationTicket(validationData, Scheme.Name);

            // return AuthenticateResult.Success
            return Task.FromResult(AuthenticateResult.Success(authTicket));
        }

        // token validation failed
        return Task.FromResult(AuthenticateResult.Fail("Unauthorized"));
    }
}
