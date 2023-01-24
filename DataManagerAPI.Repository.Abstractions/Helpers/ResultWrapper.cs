using DataManagerAPI.Repository.Abstractions.Constants;
using System.Runtime.Serialization;

namespace DataManagerAPI.Repository.Abstractions.Helpers;

[DataContract]
public class ResultWrapper<T>
{
    [DataMember(Order = 1)]
    public T? Data { get; set; }

    [DataMember(Order = 2)]
    public bool Success { get; set; }

    [DataMember(Order = 3)]
    public string? Message { get; set; }

    [DataMember(Order = 4)]
    public int StatusCode { get; set; } = StatusCodes.Status200OK;
}
