﻿using DataManagerAPI.Repository.Abstractions.Interfaces;
using System.ServiceModel;

namespace DataManagerAPI.Repository.Abstractions.gRPCInterfaces;

[ServiceContract]
public interface IgRPCUserDataRepository : IUserDataRepository
{
}