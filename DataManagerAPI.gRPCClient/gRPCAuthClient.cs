﻿using DataManagerAPI.gRPC.Abstractions.gRPCInterfaces;
using DataManagerAPI.gRPC.Abstractions.gRPCRequests;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using Grpc.Net.Client;
using ProtoBuf.Grpc.Client;

namespace DataManagerAPI.gRPCClient;

/// <summary>
/// Implementation of <see cref="IAuthRepository"/> for gRPC client.
/// </summary>
public class gRPCAuthClient : IAuthRepository
{
    private readonly IgRPCAuthRepository _igRPCAuthRepository;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="channel"><see cref="GrpcChannel"/></param>
    public gRPCAuthClient(GrpcChannel channel)
    {
        _igRPCAuthRepository = channel.CreateGrpcService<IgRPCAuthRepository>();
    }

    /// <inheritdoc />
    public Task<ResultWrapper<UserCredentialsData>> GetUserDetailsByIdAsync(int userId,
        CancellationToken cancellationToken = default)
    {
        return _igRPCAuthRepository.GetUserDetailsByIdAsync(new Int32Request { Value = userId },
            gRPCClientsHelper.CreateCallOptions(cancellationToken));
    }

    /// <inheritdoc />
    public Task<ResultWrapper<UserCredentialsData>> GetUserDetailsByLoginAsync(string login,
        CancellationToken cancellationToken = default)
    {
        return _igRPCAuthRepository.GetUserDetailsByLoginAsync(new StringRequest { Value = login },
            gRPCClientsHelper.CreateCallOptions(cancellationToken));
    }

    /// <inheritdoc />
    public Task<ResultWrapper<int>> LoginAsync(string login, UserCredentials credentials,
        CancellationToken cancellationToken = default)
    {
        return _igRPCAuthRepository.LoginAsync(new LoginRequest { Login = login, Credentials = credentials },
            gRPCClientsHelper.CreateCallOptions(cancellationToken));
    }

    /// <inheritdoc />
    public Task<ResultWrapper<int>> RefreshTokenAsync(int userId, string? refreshToken,
        CancellationToken cancellationToken = default)
    {
        return _igRPCAuthRepository.RefreshTokenAsync(new RefreshTokenRequest { UserId = userId, RefreshToken = refreshToken },
            gRPCClientsHelper.CreateCallOptions(cancellationToken));
    }

    /// <inheritdoc />
    public Task<ResultWrapper<User>> RegisterUserAsync(User userToAdd, UserCredentials userCredentials,
        CancellationToken cancellationToken = default)
    {
        return _igRPCAuthRepository.RegisterUserAsync(new RegisterUserRequest { User = userToAdd, UserCredentials = userCredentials },
            gRPCClientsHelper.CreateCallOptions(cancellationToken));
    }

    /// <inheritdoc />
    public Task<ResultWrapper<int>> UpdateUserPasswordAsync(int userId, UserCredentials credentials,
        CancellationToken cancellationToken = default)
    {
        return _igRPCAuthRepository.UpdateUserPasswordAsync(new UpdateUserPasswordRequest { UserId = userId, UserCredentials = credentials },
            gRPCClientsHelper.CreateCallOptions(cancellationToken));
    }

    /// <inheritdoc />
    public Task<ResultWrapper<RoleIds>> UpdateUserRoleAsync(int userId, RoleIds newRole,
        CancellationToken cancellationToken = default)
    {
        return _igRPCAuthRepository.UpdateUserRoleAsync(new UpdateUserRoleRequest { UserId = userId, Role = newRole },
            gRPCClientsHelper.CreateCallOptions(cancellationToken));
    }
}
