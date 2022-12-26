using DataManagerAPI.Dto;
using DataManagerAPI.Helpers;
using DataManagerAPI.Services;
using System.Security.Claims;

namespace DataManagerAPI.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var clames = CredentialsHelper.ValidateToken(token!);
            if (clames != null)
            {
                UserDto user = CredentialsHelper.CreateUser(clames);

                // attach user to context on successful jwt validation
                context.Items["User"] = user;
                context.Items["Login"] = clames.First(x => x.Type == "Login");
            }

            await _next(context);
        }
    }
}
