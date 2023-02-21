using DataManagerAPI.Repository.Abstractions.Constants;
using DataManagerAPI.Repository.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace DataManagerAPI.SQLServerDB;

/// <summary>
/// Database context.
/// </summary>
public class UsersDBContext : DbContext
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options"><see cref="DbContextOptions"/></param>
    public UsersDBContext(DbContextOptions<UsersDBContext> options) : base(options)
    {
    }

    /// <summary>
    /// Constructor for inherited class.
    /// </summary>
    /// <param name="options"><see cref="DbContextOptions"/></param>
    protected UsersDBContext(DbContextOptions options) : base(options)
    {
    }

    /// <summary>
    /// Users table
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// UserData table
    /// </summary>
    public DbSet<UserData> UserData { get; set; }

    /// <summary>
    /// UserCredentials table
    /// </summary>
    public DbSet<UserCredentials> UserCredentials { get; set; }

    /// <summary>
    /// UserFiles table
    /// </summary>
    public DbSet<UserFile> UserFiles { get; set; }

    /// <summary>
    /// Configuring database.
    /// </summary>
    /// <param name="optionsBuilder"><see cref="DbContextOptionsBuilder"/></param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //base.OnConfiguring(optionsBuilder); // base implementation is empty
        optionsBuilder.UseSqlServer(MigrationExtensions.GetConnectionString(SourceDatabases.SQLConnectionString));
    }

    /// <summary>
    /// Creation of models in database.
    /// </summary>
    /// <param name="modelBuilder"></param>
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

        // Create default user with "Admin" role.

        var defaultAdmin = new User
        {
            Id = 1,
            FirstName = "DefaultAdmin",
            LastName = "DefaultAdmin",
            Role = RoleIds.Admin
        };

        modelBuilder.Entity<User>().
            HasData(defaultAdmin);


        // Create credentials for default user.

        using var hmac = new HMACSHA512();
        var adminCredentials = new UserCredentials
        {
            UserId = 1,
            Login = "Admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin")
        };

        modelBuilder.Entity<UserCredentials>()
            .HasData(adminCredentials);
    }
}
