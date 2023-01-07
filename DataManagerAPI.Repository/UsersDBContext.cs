using DataManagerAPI.Repository.Helpers;
using DataManagerAPI.Repository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace DataManagerAPI.Repository;

public class UsersDBContext : DbContext
{
    private readonly string _connectionstring;

    public UsersDBContext(DbContextOptions<UsersDBContext> options) : base(options)
    {
        string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!;

        // Build config
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
        builder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false);

        IConfigurationRoot config = builder.Build();
        _connectionstring = config.GetConnectionString("DefaultConnection")!;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserData> UserData { get; set; }
    public DbSet<UserCredentials> UserCredentials { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer(_connectionstring);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<User>().OwnsOne(user => user.UserCredentials, builder =>
        //    {
        //        builder.ToJson();
        //    });

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

        modelBuilder.Entity<Role>()
            .HasData(RolesHelper.GetAllRoles());

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
            PasswordSalt = hmac.Key,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Admin"))
        };

        modelBuilder.Entity<UserCredentials>()
            .HasData(adminCredentials);
    }

}
