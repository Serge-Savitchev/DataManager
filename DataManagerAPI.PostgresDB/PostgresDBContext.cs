using DataManagerAPI.Repository.Abstractions.Constants;
using DataManagerAPI.SQLServerDB;
using Microsoft.EntityFrameworkCore;

namespace DataManagerAPI.PostgresDB;

/// <summary>
/// Postgres database context.
/// </summary>
public class PostgresDBContext : UsersDBContext
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options"><see cref="DbContextOptions"/></param>
    public PostgresDBContext(DbContextOptions<PostgresDBContext> options) : base(options)
    {
    }

    /// <summary>
    /// Configuring Postgres database.
    /// </summary>
    /// <param name="optionsBuilder"><see cref="DbContextOptionsBuilder"/></param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(MigrationExtensions.GetConnectionString(SourceDatabases.PostgresConnectionString));
    }

}
