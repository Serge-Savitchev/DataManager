using DataManagerAPI.Repository.Abstractions.Constants;
using DataManagerAPI.SQLServerDB;
using Microsoft.EntityFrameworkCore;

namespace DataManagerAPI.PostgresDB;

public class PostgresDBContext : UsersDBContext
{
    public PostgresDBContext(DbContextOptions<PostgresDBContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(MigrationExtensions.GetConnectionString(SourceDatabases.PostgresConnectionString));
    }

}
