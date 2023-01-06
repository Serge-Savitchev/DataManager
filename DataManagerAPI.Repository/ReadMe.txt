Migration:
PS> cd D:\Work\Source\Learning\DataManagerAPI
PS> dotnet ef migrations add Initial --project .\DataManagerAPI.Repository\DataManagerAPI.Repository.csproj --startup-project .\DataManagerAPI\DataManagerAPI.csproj
PS> dotnet ef database update --project .\DataManagerAPI.Repository\DataManagerAPI.Repository.csproj --startup-project .\DataManagerAPI\DataManagerAPI.csproj