using AutoMapper;
using DataManagerAPI.Constants;
using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using DataManagerAPI.Services.Interfaces;
using System.Security.Claims;

namespace DataManagerAPI.Services;

/// <summary>
/// Implementation of <see cref="IAuthService"/>.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IAuthRepository _repository;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;
    private readonly IUserPasswordService _userPasswordService;
    private readonly ILoggedOutUsersCollectionService _loggedOutUsersCollectionService;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="repository"><see cref="IAuthRepository"/></param>
    /// <param name="mapper"><see cref="IMapper"/></param>
    /// <param name="tokenService"><see cref="ITokenService"/></param>
    /// <param name="userPasswordService"><see cref="IUserPasswordService"/></param>
    /// <param name="loggedOutUsersCollectionService"><see cref="ILoggedOutUsersCollectionService"/></param>
    public AuthService(IAuthRepository repository,
        IMapper mapper,
        ITokenService tokenService,
        IUserPasswordService userPasswordService,
        ILoggedOutUsersCollectionService loggedOutUsersCollectionService)
    {
        _repository = repository;
        _mapper = mapper;
        _tokenService = tokenService;
        _userPasswordService = userPasswordService;
        _loggedOutUsersCollectionService = loggedOutUsersCollectionService;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserDto>> RegisterUser(RegisterUserDto userToAdd
        , CancellationToken cancellationToken = default)
    {
        UserCredentials userCredentials = _userPasswordService.CreatePasswordHash(userToAdd.Password);
        userCredentials.Login = userToAdd.Login;

        var result = await _repository.RegisterUserAsync(_mapper.Map<User>(userToAdd), userCredentials, cancellationToken);

        var ret = new ResultWrapper<UserDto>
        {
            Success = result.Success,
            Data = result.Success ? _mapper.Map<UserDto>(result.Data) : null,
            Message = result.Message,
            StatusCode = result.StatusCode
        };

        return ret;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<LoginUserResponseDto>> Login(LoginUserDto loginData
        , CancellationToken cancellationToken = default)
    {
        var result = new ResultWrapper<LoginUserResponseDto>
        {
            Success = false
        };

        ResultWrapper<UserCredentialsData> userDetails = await _repository.GetUserDetailsByLoginAsync(loginData.Login
            , cancellationToken);
        if (!userDetails.Success)
        {
            result.Message = userDetails.Message;
            result.StatusCode = userDetails.StatusCode;

            return result;
        }

        if (!_userPasswordService
            .VerifyPasswordHash(loginData.Password, userDetails.Data!.Credentials!.PasswordHash))
        {
            result.Message = "Invalide login or password";
            result.StatusCode = StatusCodes.Status401Unauthorized;
            return result;
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimNames.UserId, userDetails.Data!.User!.Id.ToString()),
            new Claim(ClaimNames.Login, loginData.Login),
            new Claim(ClaimNames.Role, Enum.GetName(typeof(RoleIds), userDetails.Data!.User!.Role)!),
            new Claim(ClaimNames.FirstName, userDetails.Data!.User!.FirstName),
            new Claim(ClaimNames.LastName, userDetails.Data!.User!.LastName),
            new Claim(ClaimNames.Email, userDetails.Data!.User!.Email ?? string.Empty)
        };

        TokenApiModelDto tokens = _tokenService.GeneratePairOfTokens(claims);

        userDetails.Data.Credentials.RefreshToken = tokens.RefreshToken;

        var loginResult = await _repository.LoginAsync(loginData.Login, userDetails.Data.Credentials, cancellationToken);
        if (!loginResult.Success)
        {
            result.Message = userDetails.Message;
            result.StatusCode = StatusCodes.Status401Unauthorized;

            return result;
        }

        result.Data = _mapper.Map<LoginUserResponseDto>(userDetails.Data.User);
        result.Data.AccessToken = tokens.AccessToken!;
        result.Data.RefreshToken = tokens.RefreshToken!;
        result.Success = true;

        _loggedOutUsersCollectionService.Remove(result.Data.Id);

        return result;
    }

    /// <inheritdoc />
    public void LogOut(int userId)
    {
        _loggedOutUsersCollectionService.Add(userId);
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<int>> Revoke(int userId, CancellationToken cancellationToken = default)
    {
        var result = await _repository.RefreshTokenAsync(userId, null, cancellationToken);
        if (result.Success)
        {
            _loggedOutUsersCollectionService.Add(userId);
        }
        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<TokenApiModelDto>> RefreshToken(TokenApiModelDto tokenData,
        CancellationToken cancellationToken = default)
    {
        var result = new ResultWrapper<TokenApiModelDto>
        {
            Success = false
        };

        ClaimsPrincipal? principal = _tokenService.ValidateToken(tokenData.AccessToken, useLifetime: false);
        if (principal is null)
        {
            result.StatusCode = StatusCodes.Status401Unauthorized;
            return result;
        }

        CurrentUserDto? user = _tokenService.CreateCurrentUser(principal.Claims);
        if (user is null)
        {
            result.StatusCode = StatusCodes.Status401Unauthorized;
            return result;
        }

        ResultWrapper<UserCredentialsData> userDetails = await _repository.GetUserDetailsByIdAsync(user.User!.Id, cancellationToken);
        if (!userDetails.Success || userDetails.Data == null)
        {
            result.StatusCode = userDetails.StatusCode;
            result.Message = userDetails.Message;
            return result;
        }

        if (userDetails.Data.Credentials!.RefreshToken != tokenData.RefreshToken)
        {
            result.StatusCode = StatusCodes.Status401Unauthorized;
            return result;
        }

        TokenApiModelDto tokens = _tokenService.GeneratePairOfTokens(principal.Claims);

        ResultWrapper<int> refreshResult = await _repository.RefreshTokenAsync(user.User!.Id, tokens.RefreshToken, cancellationToken);

        result.Success = refreshResult.Success;
        result.Message = refreshResult.Message;
        result.StatusCode = refreshResult.StatusCode;
        result.Data = new TokenApiModelDto { AccessToken = tokens.AccessToken, RefreshToken = tokens.RefreshToken };

        if (result.Success)
        {
            _loggedOutUsersCollectionService.Add(user.User!.Id);
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<int>> UpdateUserPassword(int userId, string newPassword, CancellationToken cancellationToken = default)
    {
        var credentials = _userPasswordService.CreatePasswordHash(newPassword);

        var result = await _repository.UpdateUserPasswordAsync(userId, credentials, cancellationToken);

        if (result.Success)
        {
            _loggedOutUsersCollectionService.Add(userId);
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<string>> UpdateUserRole(int userId, string newRole, CancellationToken cancellationToken = default)
    {
        var result = new ResultWrapper<string>
        {
            Success = true
        };

        try
        {
            var res = await _repository.UpdateUserRoleAsync(userId, Enum.Parse<RoleIds>(newRole, true), cancellationToken);
            if (!res.Success)
            {
                result.Success = false;
                result.StatusCode = res.StatusCode;
                result.Message = res.Message;

                return result;
            }
            result.Data = res.Data.ToString();
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = StatusCodes.Status500InternalServerError;
        }

        if (result.Success)
        {
            _loggedOutUsersCollectionService.Add(userId);
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserDetailsDto>> GetUserDetails(int userId, CancellationToken cancellationToken = default)
    {
        var userDetails = await _repository.GetUserDetailsByIdAsync(userId, cancellationToken);

        var result = new ResultWrapper<UserDetailsDto>
        {
            Success = userDetails.Success,
            Message = userDetails.Message,
            StatusCode = userDetails.StatusCode
        };

        if (userDetails.Success)
        {
            result.Data = _mapper.Map<UserDetailsDto>(userDetails.Data!.User);
            result.Data.Login = userDetails.Data.Credentials!.Login;
        }

        return result;
    }
}
