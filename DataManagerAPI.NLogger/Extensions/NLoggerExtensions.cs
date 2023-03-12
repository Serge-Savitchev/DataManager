using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
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
    public static void SetupNLogConfiguration(this WebApplicationBuilder webBuilder)
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
        //var configuration = LogManager.Configuration;
        //var consoleTarget = configuration.FindTargetByName<ConsoleTarget>("logconsole");
        //var layout = consoleTarget.Layout;
        //var text = layout.ToString();
        //consoleTarget.Layout = text;

        //LogManager.Configuration = configuration;
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
