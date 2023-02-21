using DataManagerAPI.Repository.Abstractions.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DataManagerAPI.PostgresDB;

/// <summary>
/// Provider of migration of PostgreSQL database.
/// </summary>
public class UsersDBContextFactory : IDesignTimeDbContextFactory<PostgresDBContext>
{
    /// <summary>
    /// Creates database context.
    /// </summary>
    /// <param name="args"></param>
    /// <returns>Database context. <see cref="PostgresDBContext"/></returns>
    public PostgresDBContext CreateDbContext(string[] args)
    {
        // Get environment
        string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!;

        // Build config
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        builder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);

        IConfigurationRoot config = builder.Build();

        // Get connection string
        var optionsBuilder = new DbContextOptionsBuilder<PostgresDBContext>();
        optionsBuilder.UseNpgsql(config.GetConnectionString(SourceDatabases.PostgresConnectionString));

        Console.WriteLine($"Environment: {environment}");
        Console.WriteLine(config.GetConnectionString(SourceDatabases.PostgresConnectionString));

        return new PostgresDBContext(optionsBuilder.Options);
    }
}
