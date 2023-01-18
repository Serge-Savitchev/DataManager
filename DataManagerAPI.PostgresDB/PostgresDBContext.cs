using DataManagerAPI.Repository.Abstractions.Constants;
using DataManagerAPI.SQLServerDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataManagerAPI.PostgresDB;

public class PostgresDBContext : UsersDBContext
{
    public PostgresDBContext(DbContextOptions<PostgresDBContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!;

        // Build config
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
        builder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false);

        IConfigurationRoot config = builder.Build();

        optionsBuilder.UseNpgsql(config.GetConnectionString(SourceDatabases.PostgresOption));
    }

}
