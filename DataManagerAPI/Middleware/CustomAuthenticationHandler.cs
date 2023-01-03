using DataManagerAPI.Dto;
using DataManagerAPI.Helpers;
using DataManagerAPI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace DataManagerAPI.Middleware
{
    public class CustomAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ITokenService _tokenService;

        public CustomAuthenticationHandler(
            ITokenService tokenService,
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _tokenService = tokenService;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            ClaimsPrincipal? validationData = _tokenService.ValidateToken(token!, useLifetime: true);

            if (validationData != null)
            {
                CurrentUserDto user = _tokenService.CreateCurrentUser(validationData.Claims);
                if (LoggedOutUsersCollection.Contains(user.User!.Id))
                {
                    return Task.FromResult(AuthenticateResult.Fail("Unauthorized"));
                }

                // attach user to context on successful jwt validation
                Context.Items["User"] = user;

                Context.User = validationData;

                var authTicket = new AuthenticationTicket(validationData, Scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(authTicket));
            }

            return Task.FromResult(AuthenticateResult.Fail("Unauthorized"));
        }
    }
}
