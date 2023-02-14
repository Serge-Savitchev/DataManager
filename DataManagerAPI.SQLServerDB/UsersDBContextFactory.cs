using DataManagerAPI.Repository.Abstractions.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DataManagerAPI.SQLServerDB;

public class UsersDBContextFactory : IDesignTimeDbContextFactory<UsersDBContext>
{
    public UsersDBContext CreateDbContext(string[] args)
    {
        // Get connection string
        var connectionString = MigrationExtensions.GetConnectionString(SourceDatabases.SQLConnectionString);

        var optionsBuilder = new DbContextOptionsBuilder<UsersDBContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new UsersDBContext(optionsBuilder.Options);
    }
}
