using DataManagerAPI.Repository.Abstractions.Constants;
using DataManagerAPI.Repository.Abstractions.Helpers;
using Microsoft.Extensions.Logging;

namespace DataManagerAPI.SQLServerDB;

/// <summary>
/// Helpers
/// </summary>
public static class Helpers
{
    /// <summary>
    /// Logs exception.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="wrapper"></param>
    /// <param name="ex"></param>
    /// <param name="logger"></param>
    public static void LogException<T1, T2>(ResultWrapper<T1> wrapper, Exception ex, ILogger<T2> logger)
    {
        wrapper.Success = false;
        wrapper.Message = ex.Message;
        wrapper.StatusCode = ResultStatusCodes.Status500InternalServerError;
        logger.LogError(ex, "{@wrapper}", wrapper);
    }

    /// <summary>
    /// Logs warning with Status404NotFound code.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="wrapper"></param>
    /// <param name="message"></param>
    /// <param name="logger"></param>
    public static void LogNotFoundWarning<T1, T2>(ResultWrapper<T1> wrapper, string message, ILogger<T2> logger)
    {
        wrapper.Success = false;
        wrapper.Message = message;
        wrapper.StatusCode = ResultStatusCodes.Status404NotFound;
        logger.LogWarning("Finished:{StatusCode},{message},message:not found", wrapper.StatusCode, message);
    }
}
