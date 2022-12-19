using DataManagerAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Reflection.Metadata;

namespace DataManagerAPI.Repository;

public class UsersDBContext : DbContext
{
    private readonly IConfiguration _configuration;
    public UsersDBContext(IConfiguration configuration, DbContextOptions<UsersDBContext> options) : base(options)
    {
        _configuration = configuration;
    }

    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer(_configuration["ConnectionStrings:DBConnection"]);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().OwnsOne(user => user.UserCredentials, builder =>
            {
                builder.ToTable("UserCredentials");
            });

        modelBuilder.Entity<User>().OwnsMany(user => user.UserData, builder =>
            {
                builder.ToTable("UserData");
            });

        modelBuilder.Entity<Role>()
            .ToTable("Roles");
        modelBuilder.Entity<Role>()
            .HasData(
                new Role
                {
                    Id = RoleId.Admin,
                    Name = Enum.GetName(typeof(RoleId), RoleId.Admin)!
                },
                new Role
                {
                    Id = RoleId.User,
                    Name = Enum.GetName(typeof(RoleId), RoleId.User)!
                },
                new Role
                {
                    Id = RoleId.AdvancedUser,
                    Name = Enum.GetName(typeof(RoleId), RoleId.AdvancedUser)!
                },
                new Role
                {
                    Id = RoleId.ReadOnlyUser,
                    Name = Enum.GetName(typeof(RoleId), RoleId.ReadOnlyUser)!
                }
            );

        modelBuilder.Entity<User>()
                    .HasOne<Role>()
                    .WithMany()
                    .HasForeignKey(p => p.Role);
    }

}
