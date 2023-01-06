using DataManagerAPI.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static DataManagerAPI.Tests.IntegrationTests.TestWebApplicationFactory;

namespace DataManagerAPI.Tests.IntegrationTests;

public class DatabaseFixture
{
    public const string ConnectionString = "Server=.;Database=TestDB;Trusted_Connection=true;Trust Server Certificate=true";

    private static readonly object _lockDB = new();
    private static bool _databaseInitialized;

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

                _databaseInitialized = true;
            }
        }
    }
}