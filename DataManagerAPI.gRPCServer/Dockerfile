#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DataManagerAPI.gRPCServer/DataManagerAPI.gRPCServer.csproj", "DataManagerAPI.gRPCServer/"]
RUN dotnet restore "DataManagerAPI.gRPCServer/DataManagerAPI.gRPCServer.csproj"
COPY . .
WORKDIR "/src/DataManagerAPI.gRPCServer"
RUN dotnet build "DataManagerAPI.gRPCServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DataManagerAPI.gRPCServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DataManagerAPI.gRPCServer.dll"]