using DataManagerAPI.Repository.Abstractions.Constants;
using System.Runtime.Serialization;

namespace DataManagerAPI.Repository.Abstractions.Helpers;

/// <summary>
/// Wrapper for results of methods. 
/// </summary>
/// <typeparam name="T"></typeparam>
[DataContract]
public class ResultWrapper<T>
{
    /// <summary>
    /// Returned data.
    /// </summary>
    [DataMember(Order = 1)]
    public T? Data { get; set; }

    /// <summary>
    /// Flag of successful execution of method.
    /// </summary>
    [DataMember(Order = 2)]
    public bool Success { get; set; }

    /// <summary>
    /// Error message.
    /// </summary>
    [DataMember(Order = 3)]
    public string? Message { get; set; }

    /// <summary>
    /// Error code.
    /// </summary>
    [DataMember(Order = 4)]
    public int StatusCode { get; set; } = ResultStatusCodes.Status200OK;
}
