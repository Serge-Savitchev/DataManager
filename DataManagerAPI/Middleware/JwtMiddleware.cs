using DataManagerAPI.Dto;
using DataManagerAPI.Services;
using System.Security.Claims;

namespace DataManagerAPI.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ITokenService _tokenService;

        public JwtMiddleware(RequestDelegate next, ITokenService tokenService)
        {
            _next = next;
            _tokenService = tokenService;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            ClaimsPrincipal? validationData = _tokenService.ValidateToken(token!, useLifetime: true);

            if (validationData != null)
            {
                context.User = validationData;
                CurrentUserDto user = _tokenService.CreateCurrentUser(validationData.Claims);

                // attach user to context on successful jwt validation
                context.Items["User"] = user;
            }

            await _next(context);
        }
    }
}
