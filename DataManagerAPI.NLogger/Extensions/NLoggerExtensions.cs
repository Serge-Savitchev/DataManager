using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using NLog.Web;

namespace DataManagerAPI.NLogger.Extensions;

/// <summary>
/// Extensions for NLog setup.
/// </summary>
public static class NLoggerExtensions
{
    // https://github.com/NLog/NLog.Extensions.Logging/wiki/NLog-configuration-with-appsettings.json
    /// <summary>
    /// Setup NLog configuration.
    /// </summary>
    /// <param name="webBuilder"><see cref="WebApplicationBuilder"/></param>
    /// <param name="updateTargetsTemplate"></param>
    public static void SetupNLogConfiguration(this WebApplicationBuilder webBuilder, Func<string, string>? updateTargetsTemplate)
    {
        // get environment
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!;

        // build config from NLog.*.json files
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("NLog.json", optional: false, reloadOnChange: true);
        builder.AddJsonFile($"NLog.{environment}.json", optional: true, reloadOnChange: true);

        IConfigurationRoot config = builder.Build();

        // read configuration from "NLog" section
        LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));

        // setup global item
        GlobalDiagnosticsContext.Set("ActivityId", GlobalActivity.Default);

        // set NLog as default log provider
        webBuilder.Logging.ClearProviders();
        // "RemoveLoggerFactoryFilter = false": don't reset the default Microsoft LoggerFactory Filter 
        webBuilder.Host.UseNLog(new NLogAspNetCoreOptions { RemoveLoggerFactoryFilter = false });
        _ = LogManager.Setup().GetCurrentClassLogger();

        // https://github.com/NLog/NLog/wiki/Configure-from-code

        LoggingConfiguration configuration = LogManager.Configuration;

        if (updateTargetsTemplate != null)
        {
            foreach (var target in configuration.AllTargets.OfType<TargetWithLayout>())
            {
                var layout = target.Layout.ToString();
                if (!string.IsNullOrEmpty(layout))
                {
                    var text = updateTargetsTemplate(layout!);
                    target.Layout = text;
                }
            }
            LogManager.Configuration = configuration;
        }
    }

    /// <summary>
    /// Setup middleware for logger.
    /// </summary>
    /// <param name="app"><see cref="WebApplication"/></param>
    public static void UseNLogConfiguration(this WebApplication app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
    }
}
