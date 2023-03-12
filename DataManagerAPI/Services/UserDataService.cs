using AutoMapper;
using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using DataManagerAPI.Services.Interfaces;
using System.Data;

namespace DataManagerAPI.Services;

/// <summary>
/// Implementation of IUserDataService.
/// </summary>
public class UserDataService : IUserDataService
{
    private readonly IUserDataRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserDataService> _logger;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="repository"><see cref="IUserDataRepository"/></param>
    /// <param name="mapper"><see cref="IMapper"/></param>
    /// <param name="logger"><see cref="ILogger"/></param>
    public UserDataService(IUserDataRepository repository, IMapper mapper, ILogger<UserDataService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserDataDto>> AddUserData(int userId, int userDataId, AddUserDataDto userDataToAdd,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:{userId},{userDataId}", userId, userDataId);

        var userData = _mapper.Map<UserData>(userDataToAdd);
        userData.UserId = userId;
        userData.Id = 0;

        var result = await _repository.AddUserDataAsync(userData, cancellationToken);

        var ret = ConvertWrapper(result);

        _logger.LogInformation("Finished");
        return ret;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserDataDto>> DeleteUserData(int userId, int userDataId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:{userId},{userDataId}", userId, userDataId);

        var result = await _repository.DeleteUserDataAsync(userId, userDataId, cancellationToken);

        var ret = ConvertWrapper(result);

        _logger.LogInformation("Finished");

        return ret;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserDataDto>> GetUserData(int userId, int userDataId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:{userId},{userDataId}", userId, userDataId);

        var result = await _repository.GetUserDataAsync(userId, userDataId, cancellationToken);

        var ret = ConvertWrapper(result);

        _logger.LogInformation("Finished");

        return ret;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserDataDto[]>> GetUserDataByUserId(int userId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:{userId}", userId);

        var result = await _repository.GetUserDataByUserIdAsync(userId, cancellationToken);
        var ret = new ResultWrapper<UserDataDto[]>()
        {
            Success = result.Success,
            Data = result.Success ? result.Data!.Select(_mapper.Map<UserDataDto>).ToArray() : null,
            Message = result.Message,
            StatusCode = result.StatusCode
        };

        _logger.LogInformation("Finished");

        return ret;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserDataDto>> UpdateUserData(int userId, int userDataId, AddUserDataDto userDataToUpdate,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:{userId},{userDataId}", userId, userDataId);

        var userData = _mapper.Map<UserData>(userDataToUpdate);
        userData.UserId = userId;
        userData.Id = userDataId;

        var result = await _repository.UpdateUserDataAsync(userData, cancellationToken);

        var ret = ConvertWrapper(result);

        _logger.LogInformation("Finished");

        return ret;
    }

    private ResultWrapper<UserDataDto> ConvertWrapper<T>(ResultWrapper<T> source)
    {
        var ret = new ResultWrapper<UserDataDto>
        {
            Success = source.Success,
            Data = source.Success ? _mapper.Map<UserDataDto>(source.Data) : null,
            Message = source.Message,
            StatusCode = source.StatusCode
        };
        return ret;
    }
}
