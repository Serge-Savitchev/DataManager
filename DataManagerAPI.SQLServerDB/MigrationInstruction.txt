Migration:

PS> $env:ASPNETCORE_ENVIRONMENT = 'Test' or 'Development' or 'Production' ...
PS> cd D:\Work\Source\Learning\DataManagerAPI\DataManagerAPI.SQLServerDB
PS> dotnet ef migrations add Initial
PS> dotnet ef database update