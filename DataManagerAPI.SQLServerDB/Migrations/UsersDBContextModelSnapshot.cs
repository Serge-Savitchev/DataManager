﻿// <auto-generated />
using DataManagerAPI.SQLServerDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DataManagerAPI.SQLServerDB.Migrations
{
    [DbContext(typeof(UsersDBContext))]
    partial class UsersDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DataManagerAPI.Repository.Abstractions.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Roles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Admin"
                        },
                        new
                        {
                            Id = 2,
                            Name = "PowerUser"
                        },
                        new
                        {
                            Id = 3,
                            Name = "User"
                        },
                        new
                        {
                            Id = 4,
                            Name = "ReadOnlyUser"
                        });
                });

            modelBuilder.Entity("DataManagerAPI.Repository.Abstractions.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<int>("OwnerId")
                        .HasColumnType("int");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Role");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            FirstName = "DefaultAdmin",
                            LastName = "DefaultAdmin",
                            OwnerId = 0,
                            Role = 1
                        });
                });

            modelBuilder.Entity("DataManagerAPI.Repository.Abstractions.Models.UserCredentials", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("UserCredentials");

                    b.HasData(
                        new
                        {
                            UserId = 1,
                            Login = "Admin",
                            PasswordHash = new byte[] { 183, 174, 122, 150, 10, 82, 208, 212, 77, 72, 34, 28, 218, 138, 100, 107, 118, 55, 51, 51, 224, 157, 123, 125, 234, 198, 42, 155, 45, 240, 18, 166, 140, 197, 98, 174, 216, 118, 101, 29, 131, 16, 54, 173, 50, 242, 41, 71, 58, 8, 89, 124, 219, 67, 208, 158, 188, 11, 190, 69, 116, 20, 130, 110 },
                            PasswordSalt = new byte[] { 117, 213, 235, 70, 251, 171, 14, 17, 97, 6, 13, 19, 47, 132, 7, 102, 199, 17, 99, 220, 98, 141, 197, 205, 45, 0, 5, 152, 199, 224, 173, 7, 222, 193, 33, 61, 79, 128, 72, 250, 131, 169, 35, 72, 114, 26, 79, 66, 143, 13, 133, 90, 63, 127, 0, 165, 214, 35, 83, 171, 19, 159, 194, 50, 27, 232, 127, 165, 133, 18, 34, 9, 187, 154, 228, 30, 163, 3, 84, 11, 112, 188, 229, 192, 217, 11, 17, 212, 253, 61, 140, 103, 203, 211, 182, 29, 18, 191, 32, 161, 92, 8, 70, 61, 158, 126, 204, 81, 32, 72, 143, 78, 82, 117, 56, 56, 221, 148, 182, 50, 28, 6, 198, 94, 27, 201, 192, 7 }
                        });
                });

            modelBuilder.Entity("DataManagerAPI.Repository.Abstractions.Models.UserData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Data")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserData");
                });

            modelBuilder.Entity("DataManagerAPI.Repository.Abstractions.Models.User", b =>
                {
                    b.HasOne("DataManagerAPI.Repository.Abstractions.Models.Role", null)
                        .WithMany()
                        .HasForeignKey("Role")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataManagerAPI.Repository.Abstractions.Models.UserCredentials", b =>
                {
                    b.HasOne("DataManagerAPI.Repository.Abstractions.Models.User", null)
                        .WithOne()
                        .HasForeignKey("DataManagerAPI.Repository.Abstractions.Models.UserCredentials", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataManagerAPI.Repository.Abstractions.Models.UserData", b =>
                {
                    b.HasOne("DataManagerAPI.Repository.Abstractions.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}