﻿// <auto-generated />
using DataManagerAPI.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DataManagerAPI.Repository.Migrations
{
    [DbContext(typeof(UsersDBContext))]
    [Migration("20230106154616_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DataManagerAPI.Repository.Models.Role", b =>
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

            modelBuilder.Entity("DataManagerAPI.Repository.Models.User", b =>
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

            modelBuilder.Entity("DataManagerAPI.Repository.Models.UserCredentials", b =>
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
                            PasswordHash = new byte[] { 181, 242, 203, 31, 89, 32, 163, 163, 215, 148, 146, 76, 140, 246, 30, 164, 26, 189, 3, 69, 33, 255, 226, 244, 18, 137, 54, 83, 28, 94, 184, 28, 51, 75, 141, 239, 228, 147, 107, 173, 101, 135, 69, 9, 253, 159, 220, 206, 29, 158, 14, 42, 223, 129, 237, 209, 203, 39, 210, 244, 64, 173, 221, 135 },
                            PasswordSalt = new byte[] { 96, 229, 128, 135, 135, 148, 52, 90, 36, 229, 179, 18, 72, 84, 192, 78, 174, 105, 194, 53, 93, 105, 19, 103, 29, 178, 70, 60, 21, 47, 142, 200, 55, 5, 232, 114, 222, 193, 19, 222, 178, 131, 249, 106, 170, 225, 132, 94, 73, 191, 219, 70, 196, 152, 219, 112, 196, 35, 43, 12, 33, 245, 42, 89, 66, 98, 60, 67, 184, 121, 23, 17, 107, 70, 93, 190, 17, 110, 165, 126, 235, 182, 24, 225, 252, 250, 229, 154, 73, 140, 175, 192, 86, 116, 79, 84, 133, 38, 210, 124, 249, 81, 152, 156, 160, 145, 22, 160, 189, 122, 25, 166, 6, 75, 95, 55, 161, 12, 212, 82, 13, 159, 244, 254, 112, 191, 169, 19 }
                        });
                });

            modelBuilder.Entity("DataManagerAPI.Repository.Models.UserData", b =>
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

            modelBuilder.Entity("DataManagerAPI.Repository.Models.User", b =>
                {
                    b.HasOne("DataManagerAPI.Repository.Models.Role", null)
                        .WithMany()
                        .HasForeignKey("Role")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataManagerAPI.Repository.Models.UserCredentials", b =>
                {
                    b.HasOne("DataManagerAPI.Repository.Models.User", null)
                        .WithOne()
                        .HasForeignKey("DataManagerAPI.Repository.Models.UserCredentials", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataManagerAPI.Repository.Models.UserData", b =>
                {
                    b.HasOne("DataManagerAPI.Repository.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}