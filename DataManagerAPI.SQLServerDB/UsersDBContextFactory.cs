using DataManagerAPI.Repository.Abstractions.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataManagerAPI.SQLServerDB;

/// <summary>
/// Provider of migration of SQL database.
/// </summary>
public class UsersDBContextFactory : IDesignTimeDbContextFactory<UsersDBContext>
{
    /// <summary>
    /// Creates database context.
    /// </summary>
    /// <param name="args"></param>
    /// <returns>Database context. <see cref="UsersDBContext"/></returns>
    public UsersDBContext CreateDbContext(string[] args)
    {
        // Get connection string
        var connectionString = MigrationExtensions.GetConnectionString(SourceDatabases.SQLConnectionString);

        var optionsBuilder = new DbContextOptionsBuilder<UsersDBContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new UsersDBContext(optionsBuilder.Options);
    }
}
