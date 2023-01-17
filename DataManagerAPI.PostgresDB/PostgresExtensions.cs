using DataManagerAPI.SQLServerDB;
using Microsoft.Extensions.DependencyInjection;

namespace DataManagerAPI.PostgresDB;

public static class PostgresExtensions
{
    public static IServiceCollection AddPostgresDBContext(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<UsersDBContext, PostgresDBContext>();
        return serviceCollection;
    }

}
