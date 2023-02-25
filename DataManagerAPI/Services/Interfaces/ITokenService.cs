using DataManagerAPI.Dto;
using System.Security.Claims;

namespace DataManagerAPI.Services.Interfaces;

/// <summary>
/// Interface for tokens processing.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates new access token.
    /// </summary>
    /// <param name="claims">Collection of claims. <see cref="Claim"/></param>
    /// <returns>New token. <see cref="string"/></returns>
    string GenerateAccessToken(IEnumerable<Claim> claims);

    /// <summary>
    /// Generates new refresh token.
    /// </summary>
    /// <returns>New refresh token. <see cref="string"/></returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Validates access token.
    /// </summary>
    /// <param name="token">Token. <see cref="string"/></param>
    /// <param name="useLifetime">Flag. True if checking of token's life time is required</param>
    /// <returns><see cref="ClaimsPrincipal"/></returns>
    ClaimsPrincipal? ValidateToken(string token, bool useLifetime);

    /// <summary>
    /// Creates current user from collection of claims.
    /// </summary>
    /// <param name="claims">Collection of claims. <see cref="Claim"/></param>
    /// <returns>Current user. <see cref="CurrentUserDto"/></returns>
    CurrentUserDto CreateCurrentUser(IEnumerable<Claim> claims);

    /// <summary>
    /// Generates new pair of tokens.
    /// </summary>
    /// <param name="claims">Collection of claims. <see cref="Claim"/></param>
    /// <returns>Pair of new tokens. <see cref="TokenApiModelDto"/></returns>
    TokenApiModelDto GeneratePairOfTokens(IEnumerable<Claim> claims);
}
