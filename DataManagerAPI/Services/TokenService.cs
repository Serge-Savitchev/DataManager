using DataManagerAPI.Constants;
using DataManagerAPI.Dto;
using DataManagerAPI.Helpers;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DataManagerAPI.Services;


public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ValidationTokenHelper.SecretKey));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var tokeOptions = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Tokens:AccessTokenLifetime"]!)),
            signingCredentials: signinCredentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        return tokenString;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    public ClaimsPrincipal? ValidateToken(string token, bool useLifetime)
    {
        if (token == null)
        {
            return null;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, ValidationTokenHelper.CreateTokenValidationParameters(useLifetime), out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            //bool expired = jwtToken.ValidTo < DateTime.UtcNow;
            return principal;
        }
        catch
        {
            // return null if validation fails
            return null;
        }
    }

    public CurrentUserDto CreateCurrentUser(IEnumerable<Claim> claims)
    {
        if (claims is null)
        {
            return null!;
        }

        var user = new UserDto
        {
            Id = int.Parse(claims.First(x => x.Type == ClaimNames.UserId).Value),
            FirstName = claims.First(x => x.Type == ClaimNames.FirstName).Value,
            LastName = claims.First(x => x.Type == ClaimNames.LastName).Value,
            Email = claims.First(x => x.Type == ClaimNames.Email).Value,
            Role = claims.First(x => x.Type == ClaimNames.Role).Value
        };

        var currentUser = new CurrentUserDto
        {
            User = user,
            Login = claims.First(x => x.Type == ClaimNames.Login).Value
        };

        return currentUser;
    }

    public TokenApiModelDto GeneratePairOfTokens(IEnumerable<Claim> claims)
    {
        var result = new TokenApiModelDto
        {
            AccessToken = GenerateAccessToken(claims),
            RefreshToken = GenerateRefreshToken()
        };

        return result;
    }

}
