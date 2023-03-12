using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace DataManagerAPI.NLogger;

/// <summary>
/// Middleware for handling Correlation Id.
/// </summary>
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private string _correlationId = string.Empty;   // current value of Correlation Id
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="next"><see cref="RequestDelegate"/></param>
    /// <param name="logger"><see cref="ILogger"/></param>
    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Handler.
    /// </summary>
    /// <param name="context"><see cref="HttpContext"/></param>
    /// <returns></returns>
    public async Task Invoke(HttpContext context)
    {
        string activityTraceId = Activity.Current!.TraceId.ToString();  // current activity TraceId

        // check if headers contain user-defined Correlation Id
        if (context.Request.Headers.TryGetValue(NLoggerConstants.CorrelationIdHeader, out var correlationId)
            && !string.IsNullOrWhiteSpace(correlationId))
        {
            // yes. write to log mapping of user-defined id to current activity TraceId.
            _correlationId = correlationId!;
            _logger.LogInformation("CorrelationId map:{correlationId} -> {traceId}", _correlationId, activityTraceId);
        }
        else
        {
            _correlationId = activityTraceId;   // use current activity TraceId as a Correlation Id
        }

        AddCorrelationIdHeaderToResponse(context); // provide writing of current Correlation Id to headers

        await _next(context);
    }

    private void AddCorrelationIdHeaderToResponse(HttpContext context)
        => context.Response.OnStarting(() =>
        {
            // add to response headers current Correlation Id
            context.Response.Headers.Add(NLoggerConstants.CorrelationIdHeader, new[] { _correlationId });
            return Task.CompletedTask;
        });
}
