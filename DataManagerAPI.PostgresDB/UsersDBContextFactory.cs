using DataManagerAPI.Repository.Abstractions.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DataManagerAPI.PostgresDB;

public class UsersDBContextFactory : IDesignTimeDbContextFactory<PostgresDBContext>
{
    public PostgresDBContext CreateDbContext(string[] args)
    {
        // Get environment
        string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!;

        // Build config
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
        builder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false);

        IConfigurationRoot config = builder.Build();

        // Get connection string
        var optionsBuilder = new DbContextOptionsBuilder<PostgresDBContext>();
        optionsBuilder.UseNpgsql(config.GetConnectionString(SourceDatabases.PostgresOption));

        Console.WriteLine($"Environment: {environment}");
        Console.WriteLine(config.GetConnectionString(SourceDatabases.PostgresOption));

        return new PostgresDBContext(optionsBuilder.Options);
    }
}
