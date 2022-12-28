using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DataManagerAPI.Helpers;

public static class ValidationTokenHelper
{
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

    public static string SecretKey { get; set; } = "0123456789ABCDEF0123456789ABCDEF";
}
