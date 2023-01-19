﻿using DataManagerAPI.Repository.Abstractions.Models;
using System.Runtime.Serialization;

namespace DataManagerAPI.gRPCRepository.Abstractions.gRPCRequests;

[DataContract]
public class RegisterUserRequest
{
    [DataMember(Order = 1)]
    public User? User { get; set; }

    [DataMember(Order = 2)]
    public UserCredentials? UserCredentials { get; set; }
}
