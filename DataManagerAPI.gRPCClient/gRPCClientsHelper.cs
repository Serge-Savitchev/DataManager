using DataManagerAPI.NLogger;
using Grpc.Core;
using System.Diagnostics;

namespace DataManagerAPI.gRPCClient;

/// <summary>
/// Helper for gRPC clients.
/// </summary>
public static class gRPCClientsHelper
{
    /// <summary>
    /// Creates CallOptions with cancellationToken and header.
    /// Header contains Activity TraceId current value.
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns><see cref="CallOptions"/></returns>
    public static CallOptions CreateCallOptions(CancellationToken cancellationToken)
    {
        var callOptions = new CallOptions(headers: new Metadata(), cancellationToken: cancellationToken);

        callOptions.Headers!.Add(NLoggerConstants.CorrelationIdHeader, Activity.Current!.TraceId.ToString());

        return callOptions;
    }
}
