using DataManagerAPI.Repository.Abstractions.Constants;
using DataManagerAPI.SQLServerDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace DataManagerAPI.Tests.IntegrationTests;

public static class DatabaseFixture
{
    private static readonly object _lockDB = new();
    private static bool _databaseInitialized;

    public static string SourceDatabase { get; set; } = string.Empty;
    private const string ProcessName = "DataManagerAPI.gRPCServer";


    public static void PrepareDatabase(CustomWebApplicationFactory<Program> factory)
    {
        lock (_lockDB)
        {
            if (!_databaseInitialized)
            {
                using (var scope = factory.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<UsersDBContext>();
                    db.Database.EnsureDeleted();
                    db.Database.Migrate();
                }

                if (string.Compare(SourceDatabase, SourceDatabases.gRPCOption, true) == 0)
                {
                    TryRunGRPCService();
                }

                _databaseInitialized = true;
            }
        }
    }

    private static void TryRunGRPCService()
    {
        Process? process = Process.GetProcessesByName(ProcessName).FirstOrDefault();
        if (process != null && !process.HasExited)
        {
            return;
        }

        var processFileName = Directory.GetCurrentDirectory() + "\\" + ProcessName + ".exe";
        var arguments = $"/K set ASPNETCORE_ENVIRONMENT=Test&{processFileName}";

        ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", arguments)
        {
            UseShellExecute = true,
            CreateNoWindow = false,
            WindowStyle = ProcessWindowStyle.Normal
        };

        Process.Start(processInfo);

        int count = 5;
        do
        {
            process = Process.GetProcessesByName(ProcessName).FirstOrDefault();
            Thread.Sleep(200);
            count--;

        } while (process == null && count > 0);

        if (process == null)
        {
            throw new Exception($"Can't start gRPC process {ProcessName}");
        }
    }

    public static void ShutdownGRPCService()
    {
        if (string.Compare(SourceDatabase, SourceDatabases.gRPCOption, true) == 0)
        {
            var process = Process.GetProcessesByName(ProcessName).FirstOrDefault();
            if (process != null && !process.HasExited)
            {
                process.Kill();
            }
        }
    }
}