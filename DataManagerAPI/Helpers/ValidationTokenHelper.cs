using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DataManagerAPI.Helpers;

/// <summary>
/// Helper for token validation.
/// </summary>
public static class ValidationTokenHelper
{
    /// <summary>
    /// Creates validation parameters for token.
    /// </summary>
    /// <param name="useLifetime">Flag for checking token's life time. If "false" the life time is ignored</param>
    /// <returns></returns>
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


    /// <summary>
    /// Global secret key for token validation.
    /// </summary>
    public static string SecretKey { get; set; } = "0123456789ABCDEF0123456789ABCDEF";
}
