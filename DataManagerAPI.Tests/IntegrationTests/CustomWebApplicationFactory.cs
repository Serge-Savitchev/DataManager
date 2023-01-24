using DataManagerAPI.Repository.Abstractions.Constants;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace DataManagerAPI.Tests.IntegrationTests;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    //public CustomWebApplicationFactory()
    //{
    //    // constructor
    //}

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        // my code
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, conf) =>
        {
            DatabaseFixture.SourceDatabase = context.Configuration.GetConnectionString(SourceDatabases.OptionName)!;
        });

        //builder.ConfigureServices(services =>
        //{
        //    var dbContextDescriptor = services.SingleOrDefault(
        //        d => d.ServiceType ==
        //            typeof(DbContextOptions<UsersDBContext>));

        //    services.Remove(dbContextDescriptor!);

        //    var dbConnectionDescriptor = services.SingleOrDefault(
        //        d => d.ServiceType ==
        //            typeof(DbConnection));

        //    services.Remove(dbConnectionDescriptor!);

        //    services.AddDbContext<UsersDBContext>((container, options) =>
        //    {
        //        options.UseSqlServer(DatabaseFixture.ConnectionString);
        //    });
        //});

        builder.UseEnvironment("Test");
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
    }
}
