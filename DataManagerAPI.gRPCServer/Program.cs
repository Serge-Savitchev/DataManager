using DataManagerAPI.gRPCServer.Implementation;
using DataManagerAPI.PostgresDB;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.SQLServerDB.Implementation;
using ProtoBuf.Grpc.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserDataRepository, UserDataRepository>();

builder.Services.AddCodeFirstGrpc();
builder.Services.AddPostgresDBContext();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    builder.WebHost.UseUrls("http://*:5281", "https://*:7181");
}

app.MapGrpcService<gRPCAuthRepository>();
app.MapGrpcService<gRPCUserRepository>();
app.MapGrpcService<gRPCUserDataRepository>();

app.Run();
