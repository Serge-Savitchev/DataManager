using DataManagerAPI.SQLServerDB;
using Microsoft.Extensions.DependencyInjection;

namespace DataManagerAPI.PostgresDB;

/// <summary>
/// Extensions for configure context for Postgres DB.
/// </summary>
public static class PostgresExtensions
{
    /// <summary>
    /// Add PostgresDBContext to service collection.
    /// </summary>
    /// <param name="serviceCollection"><see cref="IServiceCollection"/></param>
    /// <returns></returns>
    public static IServiceCollection AddPostgresDBContext(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<UsersDBContext, PostgresDBContext>();
        return serviceCollection;
    }

}
