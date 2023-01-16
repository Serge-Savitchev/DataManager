using DataManagerAPI.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace DataManagerAPI.PostgresDB;

public static class PostgresExtensions
{
    public static IServiceCollection AddDatabaseContext(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<UsersDBContext, PostgresDBContext>();
        return serviceCollection;
    }

}
