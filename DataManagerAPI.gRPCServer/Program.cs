using DataManagerAPI.gRPCServer.Implementation;
using DataManagerAPI.PostgresDB;
using DataManagerAPI.PostgresDB.Implementation;
using DataManagerAPI.Repository.Abstractions.Constants;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.SQLServerDB;
using DataManagerAPI.SQLServerDB.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProtoBuf.Grpc.Configuration;
using ProtoBuf.Grpc.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserDataRepository, UserDataRepository>();

string sourceDatabaseType = builder.Configuration.GetConnectionString(SourceDatabases.DatabaseType) ?? SourceDatabases.DatabaseTypeValueSQL;
if (string.Compare(sourceDatabaseType, SourceDatabases.DatabaseTypeValueSQL, true) == 0)
{
    builder.Services.AddScoped<IUserFileRepository, UserFileRepository>();
    builder.Services.AddSQLServerDBContext();   // context for SQL database
}
else if (string.Compare(sourceDatabaseType, SourceDatabases.DatabaseTypeValuePostgres, true) == 0)
{
    builder.Services.AddScoped<IUserFileRepository, UserFileRepositoryPostgres>();
    builder.Services.AddPostgresDBContext();    // context for Postgres database
}
else
{
    throw new Exception("Unknown configuration");
}


builder.Services.AddCodeFirstGrpc();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Test"))
{
    builder.WebHost.UseUrls("http://*:5281", "https://*:7181");
}

app.MapGrpcService<gRPCAuthRepository>();
app.MapGrpcService<gRPCUserRepository>();
app.MapGrpcService<gRPCUserDataRepository>();

Console.WriteLine("Arguments:");

foreach (var argument in Environment.GetCommandLineArgs())
{
    Console.WriteLine(argument);
}

Console.WriteLine($"Environment: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");

app.Run();
