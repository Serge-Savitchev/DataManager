using DataManagerAPI.Repository.Abstractions.Constants;
using DataManagerAPI.Repository.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace DataManagerAPI.SQLServerDB;

public class UsersDBContext : DbContext
{
    public UsersDBContext(DbContextOptions<UsersDBContext> options) : base(options)
    {
    }
    protected UsersDBContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserData> UserData { get; set; }
    public DbSet<UserCredentials> UserCredentials { get; set; }
    public DbSet<UserFile> UserFiles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //base.OnConfiguring(optionsBuilder); // base implementation is empty

        string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!;

        // Build config
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        builder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);

        IConfigurationRoot config = builder.Build();

        optionsBuilder.UseSqlServer(config.GetConnectionString(SourceDatabases.SQLConnectionString));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserCredentials>()
            .HasOne<User>()
            .WithOne()
            .HasForeignKey<UserCredentials>(p => p.UserId);

        modelBuilder.Entity<UserData>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<Role>()
            .ToTable("Roles");

        var roles = new List<Role>();
        foreach (RoleIds s in Enum.GetValues(typeof(RoleIds)))
        {
            roles.Add(new Role { Id = s, Name = s.ToString() });
        }

        modelBuilder.Entity<Role>()
            .HasData(roles);

        modelBuilder.Entity<User>()
                    .HasOne<Role>()
                    .WithMany()
                    .HasForeignKey(p => p.Role);

        User defaultAdmin = new User
        {
            Id = 1,
            FirstName = "DefaultAdmin",
            LastName = "DefaultAdmin",
            Role = RoleIds.Admin
        };

        modelBuilder.Entity<User>().
            HasData(defaultAdmin);

        using var hmac = new HMACSHA512();
        UserCredentials adminCredentials = new UserCredentials
        {
            UserId = 1,
            Login = "Admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin")
        };

        modelBuilder.Entity<UserCredentials>()
            .HasData(adminCredentials);
    }

}
