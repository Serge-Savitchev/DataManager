using DataManagerAPI.Constants;
using DataManagerAPI.Dto;
using DataManagerAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DataManagerAPI.Helpers;

public static class CredentialsHelper
{
    public static UserCredentials CreatePasswordHash(string password)
    {
        var result = new UserCredentials();

        if (!string.IsNullOrWhiteSpace(password))
        {
            using var hmac = new HMACSHA512();
            result.PasswordSalt = hmac.Key;
            result.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        return result;
    }

    public static string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var tokeOptions = new JwtSecurityToken(
            //issuer: "https://localhost:5001",
            //audience: "https://localhost:5001",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: signinCredentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        return tokenString;
    }

    public static CurrentUserDto CreateCurrentUser(IEnumerable<Claim> claims)
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

    public static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    public static ClaimsPrincipal? ValidateToken(string token, bool useLifetime)
    {
        if (token == null)
        {
            return null;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, CreateTokenValidationParameters(useLifetime), out SecurityToken validatedToken);

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

    public static TokenValidationParameters CreateTokenValidationParameters(bool useLifetime)
    {
        var key = Encoding.ASCII.GetBytes(SecretKey);
        var result = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = false,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = useLifetime
        };

        return result;
    }

    public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512(passwordSalt))
        {
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }

    private static string SecretKey = "0123456789ABCDEF0123456789ABCDEF";
}
