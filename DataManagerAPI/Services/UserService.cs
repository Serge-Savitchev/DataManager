using AutoMapper;
using DataManagerAPI.Dto;
using DataManagerAPI.Helpers;
using DataManagerAPI.Models;
using DataManagerAPI.Repository;
using System.Security.Claims;

namespace DataManagerAPI.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ResultWrapper<UserDto>> RegisterUser(RegisterUserDto userToAdd)
    {
        UserCredentials userCredentials = CredentialsHelper.CreatePasswordHash(userToAdd.Password);
        userCredentials.Login = userToAdd.Login;

        var result = await _repository.RegisterUser(_mapper.Map<User>(userToAdd), userCredentials);

        var ret = ConvertWrapper(result);

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

        if (!CredentialsHelper.VerifyPasswordHash(loginData.Password, user.Data!.Credentials!.PasswordHash, user.Data!.Credentials!.PasswordSalt))
        {
            result.Message = "Invalide login or password";
            result.StatusCode = StatusCodes.Status401Unauthorized;
            return result;
        }

        var claims = new List<Claim>
        {
            new Claim("UserId", user.Data!.User!.Id.ToString()),
            new Claim("Login", loginData.Login),
            new Claim("Role", Enum.GetName(typeof(RoleId), user.Data!.User!.Role)!),
            new Claim("FirstName", user.Data!.User!.FirstName),
            new Claim("LastName", user.Data!.User!.LastName),
            new Claim("Email", user.Data!.User!.Email ?? string.Empty)
        };

        var accessToken = CredentialsHelper.GenerateAccessToken(claims);

        var credentials = new UserCredentials
        {
            RefreshToken = CredentialsHelper.GenerateRefreshToken()
        };

        var loginResult = await _repository.Login(loginData.Login, credentials);
        if (!loginResult.Success)
        {
            result.Message = user.Message;
            result.StatusCode = StatusCodes.Status401Unauthorized;

            return result;
        }

        result.Data = _mapper.Map<LoginUserResponseDto>(user.Data.User);
        result.Data.Token = accessToken;
        result.Data.RefreshToken = credentials.RefreshToken;
        result.Success = true;

        return result;
    }

    public async Task<ResultWrapper<UserDto>> DeleteUser(int userId)
    {
        var result = await _repository.DeleteUser(userId);

        var ret = ConvertWrapper(result);

        return ret;
    }

    public async Task<ResultWrapper<List<UserDto>>> GetAllUsers()
    {
        var result = await _repository.GetAllUsers();
        var ret = new ResultWrapper<List<UserDto>>()
        {
            Success = result.Success,
            Data = result.Success ? result.Data!.Select(_mapper.Map<UserDto>).ToList() : null,
            Message = result.Message,
            StatusCode = result.StatusCode
        };
        return ret;
    }

    public async Task<ResultWrapper<UserDto>> GetUser(int userId)
    {
        var result = await _repository.GetUser(userId);

        var ret = ConvertWrapper(result);

        return ret;
    }

    public async Task<ResultWrapper<List<UserDto>>> GetUsersByRole(string role)
    {
        var result = await _repository.GetUsersByRole(Enum.Parse<RoleId>(role, true));
        var ret = new ResultWrapper<List<UserDto>>()
        {
            Success = result.Success,
            Data = result.Success ? result.Data!.Select(_mapper.Map<UserDto>).ToList() : null,
            Message = result.Message,
            StatusCode = result.StatusCode
        };
        return ret;
    }

    //public async Task<ResultWrapper<UserDto>> UpdateUser(UserDto userToUpdate)
    //{
    //    var result = await _repository.UpdateUser(_mapper.Map<User>(userToUpdate));

    //    var ret = ConvertWrapper(result);

    //    return ret;
    //}

    public async Task<ResultWrapper<string>> UpdateUserPassword(int userId, string newPassword)
    {
        var credentials = CredentialsHelper.CreatePasswordHash(newPassword);

        var result = await _repository.UpdateUserPassword(userId, credentials);

        return new ResultWrapper<string>
        {
            Success = result.Success,
            StatusCode = result.StatusCode,
            //Data = /*result?.Data != null ? result?.Data.UserCredentials.Login :*/ null,
            Message = result?.Message
        };
    }

    public async Task<ResultWrapper<string>> GetUserCredentials(int userId)
    {
        var result = await _repository.GetUserCredentials(userId);

        return new ResultWrapper<string>
        {
            Success = result.Success,
            StatusCode = result.StatusCode,
            Data = result?.Data != null ? result?.Data.Login : null,
            Message = result?.Message
        };
    }

    private ResultWrapper<UserDto> ConvertWrapper<T>(ResultWrapper<T> source)
    {
        var ret = new ResultWrapper<UserDto>
        {
            Success = source.Success,
            Data = source.Success ? _mapper.Map<UserDto>(source.Data) : null,
            Message = source.Message,
            StatusCode = source.StatusCode
        };
        return ret;
    }
}
