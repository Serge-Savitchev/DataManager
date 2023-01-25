using AutoMapper;
using DataManagerAPI.Constants;
using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using System.Security.Claims;

namespace DataManagerAPI.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _repository;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;
    private readonly IUserPasswordService _userPasswordService;
    private readonly ILoggedOutUsersCollectionService _loggedOutUsersCollectionService;

    public AuthService(IAuthRepository repository, IMapper mapper,
        ITokenService tokenService, IUserPasswordService userPasswordService, ILoggedOutUsersCollectionService loggedOutUsersCollectionService)
    {
        _repository = repository;
        _mapper = mapper;
        _tokenService = tokenService;
        _userPasswordService = userPasswordService;
        _loggedOutUsersCollectionService = loggedOutUsersCollectionService;
    }

    public async Task<ResultWrapper<UserDto>> RegisterUser(RegisterUserDto userToAdd)
    {
        UserCredentials userCredentials = _userPasswordService.CreatePasswordHash(userToAdd.Password);
        userCredentials.Login = userToAdd.Login;

        var result = await _repository.RegisterUserAsync(_mapper.Map<User>(userToAdd), userCredentials);

        var ret = new ResultWrapper<UserDto>
        {
            Success = result.Success,
            Data = result.Success ? _mapper.Map<UserDto>(result.Data) : null,
            Message = result.Message,
            StatusCode = result.StatusCode
        };

        return ret;
    }

    public async Task<ResultWrapper<LoginUserResponseDto>> Login(LoginUserDto loginData)
    {
        var result = new ResultWrapper<LoginUserResponseDto>
        {
            Success = false
        };

        ResultWrapper<UserCredentialsData> userDetails = await _repository.GetUserDetailsByLoginAsync(loginData.Login);
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

        var loginResult = await _repository.LoginAsync(loginData.Login, userDetails.Data.Credentials);
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

    public void LogOut(int userId)
    {
        _loggedOutUsersCollectionService.Add(userId);
    }

    public async Task<ResultWrapper<int>> Revoke(int userId)
    {
        var result = await _repository.RefreshTokenAsync(userId, null);
        if (result.Success)
        {
            _loggedOutUsersCollectionService.Add(userId);
        }
        return result;
    }

    public async Task<ResultWrapper<TokenApiModelDto>> RefreshToken(TokenApiModelDto tokenData)
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

        ResultWrapper<UserCredentialsData> userDetails = await _repository.GetUserDetailsByIdAsync(user.User!.Id);
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

        ResultWrapper<int> refreshResult = await _repository.RefreshTokenAsync(user.User!.Id, tokens.RefreshToken);

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

    public async Task<ResultWrapper<int>> UpdateUserPassword(int userId, string newPassword)
    {
        var credentials = _userPasswordService.CreatePasswordHash(newPassword);

        var result = await _repository.UpdateUserPasswordAsync(userId, credentials);

        if (result.Success)
        {
            _loggedOutUsersCollectionService.Add(userId);
        }

        return result;
    }

    public async Task<ResultWrapper<string>> UpdateUserRole(int userId, string newRole)
    {
        var result = new ResultWrapper<string>
        {
            Success = true
        };

        try
        {
            var res = await _repository.UpdateUserRoleAsync(userId, Enum.Parse<RoleIds>(newRole, true));
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

    public async Task<ResultWrapper<UserDetailsDto>> GetUserDetails(int userId)
    {
        var userDetails = await _repository.GetUserDetailsByIdAsync(userId);

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
