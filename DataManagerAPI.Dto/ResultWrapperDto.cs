using Microsoft.AspNetCore.Http;

namespace DataManagerAPI.Dto;

/// <summary>
/// Wrapper for results of methods. 
/// </summary>
/// <typeparam name="T"></typeparam>
public class ResultWrapperDto<T>
{
    /// <summary>
    /// Returned data.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Flag of successful execution of method.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message.
    /// </summary>
    public string? Message { get; set; }
    /// <summary>
    /// Error code.
    /// </summary>
    public int StatusCode { get; set; } = StatusCodes.Status200OK;

}
