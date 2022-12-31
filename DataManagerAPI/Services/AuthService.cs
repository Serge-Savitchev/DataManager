using AutoMapper;
using DataManagerAPI.Constants;
using DataManagerAPI.Dto;
using DataManagerAPI.Helpers;
using DataManagerAPI.Models;
using DataManagerAPI.Repository;
using System.Security.Claims;

namespace DataManagerAPI.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _repository;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;
    private readonly IUserPasswordService _userPasswordService;

    public AuthService(IAuthRepository repository, IMapper mapper,
        ITokenService tokenService, IUserPasswordService userPasswordService)
    {
        _repository = repository;
        _mapper = mapper;
        _tokenService = tokenService;
        _userPasswordService = userPasswordService;
    }

    public async Task<ResultWrapper<UserDto>> RegisterUser(RegisterUserDto userToAdd)
    {
        UserCredentials userCredentials = _userPasswordService.CreatePasswordHash(userToAdd.Password);
        userCredentials.Login = userToAdd.Login;

        var result = await _repository.RegisterUser(_mapper.Map<User>(userToAdd), userCredentials);

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

        var user = await _repository.GetUserByLogin(loginData.Login);
        if (!user.Success)
        {
            result.Message = user.Message;
            result.StatusCode = user.StatusCode;

            return result;
        }

        if (!_userPasswordService.VerifyPasswordHash(loginData.Password, user.Data!.Credentials!.PasswordHash, user.Data!.Credentials!.PasswordSalt))
        {
            result.Message = "Invalide login or password";
            result.StatusCode = StatusCodes.Status401Unauthorized;
            return result;
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimNames.UserId, user.Data!.User!.Id.ToString()),
            new Claim(ClaimNames.Login, loginData.Login),
            new Claim(ClaimNames.Role, Enum.GetName(typeof(RoleIds), user.Data!.User!.Role)!),
            new Claim(ClaimNames.FirstName, user.Data!.User!.FirstName),
            new Claim(ClaimNames.LastName, user.Data!.User!.LastName),
            new Claim(ClaimNames.Email, user.Data!.User!.Email ?? string.Empty)
        };

        TokenApiModelDto tokens = _tokenService.GeneratePairOfTokens(claims);

        var credentials = new UserCredentials
        {
            RefreshToken = tokens.RefreshToken
        };

        var loginResult = await _repository.Login(loginData.Login, credentials);
        if (!loginResult.Success)
        {
            result.Message = user.Message;
            result.StatusCode = StatusCodes.Status401Unauthorized;

            return result;
        }

        result.Data = _mapper.Map<LoginUserResponseDto>(user.Data.User);
        result.Data.AccessToken = tokens.AccessToken!;
        result.Data.RefreshToken = tokens.RefreshToken!;
        result.Success = true;

        return result;
    }

    public async Task<ResultWrapper<int>> Revoke(int userId)
    {
        var result = await _repository.RefreshToken(userId, null);
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

        ResultWrapper<UserCredentials> credentials = await _repository.GetUserCredentials(user.User!.Id);
        if (!credentials.Success || credentials.Data == null)
        {
            result.StatusCode = credentials.StatusCode;
            result.Message = credentials.Message;
            return result;
        }

        if (credentials.Data.RefreshToken != tokenData.RefreshToken)
        {
            result.StatusCode = StatusCodes.Status401Unauthorized;
            return result;
        }

        TokenApiModelDto tokens = _tokenService.GeneratePairOfTokens(principal.Claims);

        ResultWrapper<int> refreshResult = await _repository.RefreshToken(user.User!.Id, tokens.RefreshToken);

        result.Success = refreshResult.Success;
        result.Message = refreshResult.Message;
        result.StatusCode = refreshResult.StatusCode;
        result.Data = new TokenApiModelDto { AccessToken = tokens.AccessToken, RefreshToken = tokens.RefreshToken };

        return result;
    }

    public async Task<ResultWrapper<int>> UpdateUserPassword(int userId, string newPassword)
    {
        var credentials = _userPasswordService.CreatePasswordHash(newPassword);

        var result = await _repository.UpdateUserPassword(userId, credentials);

        return result;
    }
}
