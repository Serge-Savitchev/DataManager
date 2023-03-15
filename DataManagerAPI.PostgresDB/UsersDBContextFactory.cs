using DataManagerAPI.Repository.Abstractions.Constants;
using DataManagerAPI.SQLServerDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

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
        // Get connection string
        var connectionString = MigrationExtensions.GetConnectionString(SourceDatabases.PostgresConnectionString);

        var optionsBuilder = new DbContextOptionsBuilder<PostgresDBContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new PostgresDBContext(optionsBuilder.Options);
    }
}
