﻿using System.Runtime.Serialization;

namespace DataManagerAPI.gRPCRepository.Abstractions.gRPCRequests;

/// <summary>
/// gRPC request.
/// </summary>
[DataContract]
public class StringRequest
{
    /// <summary>
    /// Value
    /// </summary>
    [DataMember(Order = 1)]
    public string Value { get; set; } = string.Empty;
}
