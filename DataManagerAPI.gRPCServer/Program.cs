using DataManagerAPI.gRPCServer.Implementation;
using DataManagerAPI.NLogger.Extensions;
using DataManagerAPI.PostgresDB;
using DataManagerAPI.PostgresDB.Implementation;
using DataManagerAPI.Repository.Abstractions.Constants;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.SQLServerDB;
using DataManagerAPI.SQLServerDB.Implementation;
using ProtoBuf.Grpc.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IUserDataRepository, UserDataRepository>();

string sourceDatabaseType = builder.Configuration.GetConnectionString(SourceDatabases.DatabaseType) ?? SourceDatabases.DatabaseTypeValueSQL;
if (string.Compare(sourceDatabaseType, SourceDatabases.DatabaseTypeValueSQL, true) == 0)
{
    builder.Services.AddScoped<IUserFilesRepository, UserFilesRepository>();
    builder.Services.AddSQLServerDBContext();   // context for SQL database
}
else if (string.Compare(sourceDatabaseType, SourceDatabases.DatabaseTypeValuePostgres, true) == 0)
{
    builder.Services.AddScoped<IUserFilesRepository, UserFileRepositoryPostgres>();
    builder.Services.AddPostgresDBContext();    // context for Postgres database
}
else
{
    throw new Exception("Unknown configuration");
}

// take port for listening from "gRPCConnectionString" value.
string gRPCserverConnectionString = builder.Configuration.GetConnectionString(SourceDatabases.gRPCConnectionString) ?? "https://localhost:7181";
int index0 = gRPCserverConnectionString.IndexOf("://") + 3;
int index1 = gRPCserverConnectionString.LastIndexOf(":");
if (index1 > index0 + 3)
{
    //  https://domen.com:443 -> https://*:443
    string useUrls = string.Concat(gRPCserverConnectionString.AsSpan(0, index0), "*", gRPCserverConnectionString.AsSpan(index1));
    builder.WebHost.UseUrls(useUrls);
}
else
{
    builder.WebHost.UseUrls("http://*:80");
}

builder.Services.AddCodeFirstGrpc(options => { options.MaxReceiveMessageSize = null; });

builder.SetupNLogConfiguration();

var app = builder.Build();

app.UseNLogConfiguration();

app.MapGrpcService<gRPCAuthRepository>();
app.MapGrpcService<gRPCUsersRepository>();
app.MapGrpcService<gRPCUserDataRepository>();
app.MapGrpcService<gRPCUserFilesRepository>();
app.MapGrpcService<gRPCProtoService>();

app.Run();
