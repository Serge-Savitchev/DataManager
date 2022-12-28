using DataManagerAPI.Dto;
using System.Security.Claims;

namespace DataManagerAPI.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        ClaimsPrincipal? ValidateToken(string token, bool useLifetime);
        CurrentUserDto CreateCurrentUser(IEnumerable<Claim> claims);
        TokenApiModelDto GeneratePairOfTokens(IEnumerable<Claim> claims);
    }
}
