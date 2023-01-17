using Microsoft.Extensions.DependencyInjection;

namespace DataManagerAPI.SQLServerDB;

public static class SQLServerExtensions
{
    public static IServiceCollection AddSQLServerDBContext(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<UsersDBContext>();
        return serviceCollection;
    }
}
