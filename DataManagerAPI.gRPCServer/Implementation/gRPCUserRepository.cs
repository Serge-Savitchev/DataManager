using DataManagerAPI.Repository.Abstractions.gRPCInterfaces;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;

namespace DataManagerAPI.gRPCServer.Implementation;

public class gRPCUserRepository : IgRPCUserRepository
{
    private readonly IUserRepository _repository;

    public gRPCUserRepository(IUserRepository repository)
    {
        _repository = repository;
    }

    public Task<ResultWrapper<User>> DeleteUser(int userId)
    {
        return _repository.DeleteUser(userId);
    }

    public Task<ResultWrapper<List<User>>> GetAllUsers()
    {
        return _repository.GetAllUsers();
    }

    public Task<ResultWrapper<User>> GetUser(int userId)
    {
        return _repository.GetUser(userId);
    }

    public Task<ResultWrapper<List<User>>> GetUsersByRole(RoleIds roleId)
    {
        return _repository.GetUsersByRole(roleId);
    }

    public Task<ResultWrapper<int>> UpdateOwners(int ownerId, int[] users)
    {
        return _repository.UpdateOwners(ownerId, users);
    }
}
