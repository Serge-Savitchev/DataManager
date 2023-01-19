using DataManagerAPI.PostgresDB;
using DataManagerAPI.Repository.Abstractions.Constants;
using DataManagerAPI.gRPCRepository.Abstractions.gRPCInterfaces;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.gRPCClients;
using DataManagerAPI.SQLServerDB;
using DataManagerAPI.SQLServerDB.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataManagerAPI.Repository;

public static class RepositoryExtensions
{
    public static IServiceCollection AddSelectedDBContext(this IServiceCollection serviceCollection,
            ConfigurationManager configuration)
    {
        string sourceDatabase = configuration.GetConnectionString(SourceDatabases.OptionName) ?? SourceDatabases.SQLServerOption;

        if (string.Compare(sourceDatabase, SourceDatabases.SQLServerOption, true) == 0)
        {
            serviceCollection.AddScoped<IAuthRepository, AuthRepository>();
            serviceCollection.AddScoped<IUserRepository, UserRepository>();
            serviceCollection.AddScoped<IUserDataRepository, UserDataRepository>();

            serviceCollection.AddSQLServerDBContext();
            return serviceCollection;
        }

        if (string.Compare(sourceDatabase, SourceDatabases.PostgresOption, true) == 0)
        {
            serviceCollection.AddScoped<IAuthRepository, AuthRepository>();
            serviceCollection.AddScoped<IUserRepository, UserRepository>();
            serviceCollection.AddScoped<IUserDataRepository, UserDataRepository>();

            serviceCollection.AddPostgresDBContext();
            return serviceCollection;
        }

        if (string.Compare(sourceDatabase, SourceDatabases.gRPCOption, true) == 0)
        {
            serviceCollection.AddScoped<IAuthRepository, gRPCAuthClient>();
            serviceCollection.AddScoped<IUserRepository, gRPCUserClient>();
            serviceCollection.AddScoped<IUserDataRepository, gRPCUserDataClient>();

            var uri = new Uri(configuration.GetConnectionString(SourceDatabases.gRPCOption)!);

            serviceCollection.AddGrpcClient<IgRPCAuthRepository>(o =>
            {
                o.Address = uri;
            });

            serviceCollection.AddGrpcClient<IgRPCUserRepository>(o =>
            {
                o.Address = uri;
            });

            serviceCollection.AddGrpcClient<IgRPCUserDataRepository>(o =>
            {
                o.Address = uri;
            });

            serviceCollection.AddPostgresDBContext();
            return serviceCollection;
        }

        throw new NotImplementedException("Unknown source for database.");
    }
}
