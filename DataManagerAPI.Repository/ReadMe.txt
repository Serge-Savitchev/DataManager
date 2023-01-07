Migration:
PS> cd D:\Work\Source\Learning\DataManagerAPI
PS> dotnet ef migrations add Initial --project .\DataManagerAPI.Repository\DataManagerAPI.Repository.csproj --startup-project .\DataManagerAPI\DataManagerAPI.csproj
PS> dotnet ef database update --project .\DataManagerAPI.Repository\DataManagerAPI.Repository.csproj --startup-project .\DataManagerAPI\DataManagerAPI.csproj

OR

PS> $env:ASPNETCORE_ENVIRONMENT = 'Test' or 'Development' or 'Production' ...
PS> cd D:\Work\Source\Learning\DataManagerAPI\DataManagerAPI.Repository
PS>  dotnet ef migrations add Initial
PS>  dotnet ef database update