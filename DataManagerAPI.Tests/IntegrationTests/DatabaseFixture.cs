using DataManagerAPI.SQLServerDB;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace DataManagerAPI.Tests.IntegrationTests;

public static class DatabaseFixture
{
    private static readonly object _lockDB = new();
    private static bool _databaseInitialized;

    public static bool UseGRPCServer { get; set; } = false;

    // name of gRPC server
    private const string ProcessName = "DataManagerAPI.gRPCServer";

    /// <summary>
    /// Initializes database for tests.
    /// </summary>
    /// <param name="factory"></param>
    public static void PrepareDatabase(CustomWebApplicationFactory<Program> factory)
    {
        lock (_lockDB) // initializing has to be called only one time
        {
            if (!_databaseInitialized)
            {
                using (var scope = factory.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<UsersDBContext>();
                    db.Database.EnsureDeleted();    // delete database
                    db.Database.Migrate();          // initialize new one
                }

                if (UseGRPCServer)  // if configuration requires gRPC server, run it
                {
                    TryRunGRPCService();
                }

                // login default admin
                var client = factory.CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });

                UsersForTestsHelper.LoginDefaultAdmin(client);

                _databaseInitialized = true;    // database is ready for tests
            }
        }
    }

    /// <summary>
    /// Starts gRPC server.
    /// </summary>
    /// <exception cref="Exception"></exception>
    private static void TryRunGRPCService()
    {
        Process? process = Process.GetProcessesByName(ProcessName).FirstOrDefault();
        if (process != null && !process.HasExited)
        {
            return; // already runs
        }

        // start server via cmd.exe

        var processFileName = Directory.GetCurrentDirectory() + "\\" + ProcessName + ".exe";
        var arguments = $"/K set ASPNETCORE_ENVIRONMENT=Test&{processFileName}";

        ProcessStartInfo processInfo = new("cmd.exe", arguments)
        {
            UseShellExecute = true,
            CreateNoWindow = false,
            WindowStyle = ProcessWindowStyle.Normal
        };

        Process.Start(processInfo);

        // waiting for server gets ready
        int count = 5;
        do
        {
            process = Process.GetProcessesByName(ProcessName).FirstOrDefault();
            Thread.Sleep(200);
            count--;

        } while (process == null && count > 0);

        if (process == null)    // something goes wrong...
        {
            throw new Exception($"Can't start gRPC process {ProcessName}");
        }
    }

    /// <summary>
    /// Stops gRPC server.
    /// </summary>
    public static void ShutdownGRPCService()
    {
        if (UseGRPCServer)
        {
            var process = Process.GetProcessesByName(ProcessName).FirstOrDefault();
            if (process != null && !process.HasExited)
            {
                process.Kill();
            }
        }
    }
}