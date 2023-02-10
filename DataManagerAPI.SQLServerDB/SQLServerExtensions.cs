using Microsoft.Extensions.DependencyInjection;

namespace DataManagerAPI.SQLServerDB;

/// <summary>
/// Extensions for DBContext
/// </summary>
public static class SQLServerExtensions
{
    /// <summary>
    /// Adds DBContext to service collection.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection AddSQLServerDBContext(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<UsersDBContext>();
        return serviceCollection;
    }
}
