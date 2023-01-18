using DataManagerAPI.Repository.Abstractions.gRPCInterfaces;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;

namespace DataManagerAPI.Repository.gRPCClients;

public class gRPCUserClient : IUserRepository
{
    private readonly IgRPCUserRepository _igRPCUserRepository;

    public gRPCUserClient(IgRPCUserRepository igRPCUserRepository)
    {
        _igRPCUserRepository = igRPCUserRepository;
    }
    public Task<ResultWrapper<User>> DeleteUser(int userId)
    {
        return _igRPCUserRepository.DeleteUser(userId);
    }

    public Task<ResultWrapper<List<User>>> GetAllUsers()
    {
        return _igRPCUserRepository.GetAllUsers();
    }

    public Task<ResultWrapper<User>> GetUser(int userId)
    {
        return _igRPCUserRepository.GetUser(userId);
    }

    public Task<ResultWrapper<List<User>>> GetUsersByRole(RoleIds roleId)
    {
        return _igRPCUserRepository.GetUsersByRole(roleId);
    }

    public Task<ResultWrapper<int>> UpdateOwners(int ownerId, int[] users)
    {
        return _igRPCUserRepository.UpdateOwners(ownerId, users);
    }
}
