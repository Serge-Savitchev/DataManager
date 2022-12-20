using DataManagerAPI.Helpers;
using DataManagerAPI.Models;

namespace DataManagerAPI.Repository;

public interface IUserDataRepository
{
    Task<ResultWrapper<UserData>> AddUserData(UserData userDataToAdd);
}
