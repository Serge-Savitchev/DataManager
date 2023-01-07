﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DataManagerAPI.Repository;

public class UsersDBContextFactory : IDesignTimeDbContextFactory<UsersDBContext>
{
    public UsersDBContext CreateDbContext(string[] args)
    {
        // Get environment
        string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!;

        // Build config
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
        builder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false);

        IConfigurationRoot config = builder.Build();

        // Get connection string
        var optionsBuilder = new DbContextOptionsBuilder<UsersDBContext>();
        optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));

        Console.WriteLine($"Environment: {environment}");
        Console.WriteLine(config.GetConnectionString("DefaultConnection"));

        return new UsersDBContext(optionsBuilder.Options);
    }
}