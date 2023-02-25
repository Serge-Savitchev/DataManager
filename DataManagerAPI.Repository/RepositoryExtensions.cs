using DataManagerAPI.PostgresDB;
using DataManagerAPI.PostgresDB.Implementation;
using DataManagerAPI.Repository.Abstractions.Constants;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.gRPCClients;
using DataManagerAPI.SQLServerDB;
using DataManagerAPI.SQLServerDB.Implementation;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataManagerAPI.Repository;

/// <summary>
/// Class for setup databases in accordance with configurations.
/// </summary>
public static class RepositoryExtensions
{
    /// <summary>
    /// Adds required services in accordance with configurations.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="configuration"></param>
    /// <returns>Service collection. <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddSelectedDBContext(this IServiceCollection serviceCollection,
            ConfigurationManager configuration)
    {
        // take type of database: SQL or Postgres
        string sourceDatabaseType = configuration.GetConnectionString(SourceDatabases.DatabaseType) ?? SourceDatabases.DatabaseTypeValueSQL;

        bool useGPRC = false;   // check if we are working via gRPC service 
        if (!bool.TryParse(configuration.GetConnectionString(SourceDatabases.UseGPRC), out useGPRC))
        {
            useGPRC = false;
        }

        if (useGPRC)    // add required services for gRPC
        {
            serviceCollection.AddScoped<IAuthRepository, gRPCAuthClient>();
            serviceCollection.AddScoped<IUsersRepository, gRPCUsersClient>();
            serviceCollection.AddScoped<IUserDataRepository, gRPCUserDataClient>();
            serviceCollection.AddScoped<IUserFilesRepository, gRPCUserFilesClient>();

            GrpcChannel channel = GrpcChannel.ForAddress(
                configuration.GetConnectionString(SourceDatabases.gRPCConnectionString)!,
                    new GrpcChannelOptions { MaxReceiveMessageSize = null });

            serviceCollection.AddSingleton(channel);
        }
        else  // add required services for direct connection to database
        {
            serviceCollection.AddScoped<IAuthRepository, AuthRepository>();
            serviceCollection.AddScoped<IUsersRepository, UsersRepository>();
            serviceCollection.AddScoped<IUserDataRepository, UserDataRepository>();

            if (string.Compare(sourceDatabaseType, SourceDatabases.DatabaseTypeValueSQL, true) == 0)
            {
                serviceCollection.AddScoped<IUserFilesRepository, UserFilesRepository>();
                serviceCollection.AddSQLServerDBContext();  // context for SQL database
            }
            else if (string.Compare(sourceDatabaseType, SourceDatabases.DatabaseTypeValuePostgres, true) == 0)
            {
                serviceCollection.AddScoped<IUserFilesRepository, UserFileRepositoryPostgres>();
                serviceCollection.AddPostgresDBContext();   // context for Postgres database
            }
        }

        return serviceCollection;
    }
}
