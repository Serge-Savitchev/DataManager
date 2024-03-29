﻿using System.Runtime.Serialization;

namespace DataManagerAPI.gRPC.Abstractions.gRPCRequests;

/// <summary>
/// gRPC request.
/// </summary>
[DataContract]
public class Int32Request
{
    /// <summary>
    /// Value
    /// </summary>
    [DataMember(Order = 1)]
    public int Value { get; set; }
}
