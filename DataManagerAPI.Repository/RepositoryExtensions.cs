using DataManagerAPI.PostgresDB;
using DataManagerAPI.Repository.Abstractions.Constants;
using DataManagerAPI.SQLServerDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataManagerAPI.Repository;

public static class RepositoryExtensions
{
    public static IServiceCollection AddSelectedDBContext(this IServiceCollection serviceCollection,
            ConfigurationManager configuration)
    {
        string sourceDatabases = configuration.GetConnectionString(SourceDatabases.OptionName) ?? SourceDatabases.SQLServerOption;
        if (string.Compare(sourceDatabases, SourceDatabases.SQLServerOption, true) == 0)
        {
            serviceCollection.AddSQLServerDBContext();
            return serviceCollection;
        }

        if (string.Compare(sourceDatabases, SourceDatabases.PostgresOption, true) == 0)
        {
            serviceCollection.AddPostgresDBContext();
            return serviceCollection;
        }

        throw new NotImplementedException("Unknown source for database.");
    }
}
