using DataManagerAPI.Helpers;
using DataManagerAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DataManagerAPI.Repository;

public class UsersDBContext : DbContext
{
    public UsersDBContext(DbContextOptions<UsersDBContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserData> UserData { get; set; }
    public DbSet<UserCredentials> UserCredentials { get; set; }

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
