using Microsoft.Extensions.DependencyInjection;

namespace DataManagerAPI.Repository;

public static class RepositoryExtensions
{
    public static IServiceCollection AddDatabaseContext(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<UsersDBContext>();
        return serviceCollection;
    }
}
