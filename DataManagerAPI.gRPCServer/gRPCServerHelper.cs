using DataManagerAPI.NLogger;
using Grpc.Core;
using ProtoBuf.Grpc;

namespace DataManagerAPI.gRPCServer;

/// <summary>
/// Helper for gRPC clients.
/// </summary>
public static class gRPCServerHelper
{
    /// <summary>
    /// Gets remote activity trace Id from header,
    /// </summary>
    /// <param name="context"><see cref="CallContext"/></param>
    /// <returns>remote activity trace Id</returns>
    public static string GetRemoteActivityTraceId(CallContext context)
    {
        var result = context.RequestHeaders?.GetValue(NLoggerConstants.CorrelationIdHeader);
        return result ?? "";
    }

    /// <summary>
    /// Gets remote activity trace Id from header,
    /// </summary>
    /// <param name="context"><see cref="CallContext"/></param>
    /// <returns>remote activity trace Id</returns>
    public static string GetRemoteActivityTraceId(ServerCallContext context)
    {
        var result = context.RequestHeaders?.GetValue(NLoggerConstants.CorrelationIdHeader);
        return result ?? "";
    }

}
