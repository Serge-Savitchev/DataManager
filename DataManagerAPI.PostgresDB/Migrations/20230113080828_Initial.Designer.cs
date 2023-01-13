﻿// <auto-generated />
using DataManagerAPI.PostgresDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataManagerAPI.PostgresDB.Migrations
{
    [DbContext(typeof(PostgresDBContext))]
    [Migration("20230113080828_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DataManagerAPI.Repository.Abstractions.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

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
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<int>("OwnerId")
                        .HasColumnType("integer");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

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
                        .HasColumnType("integer");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("text");

                    b.HasKey("UserId");

                    b.ToTable("UserCredentials");

                    b.HasData(
                        new
                        {
                            UserId = 1,
                            Login = "Admin",
                            PasswordHash = new byte[] { 89, 201, 15, 23, 149, 5, 10, 74, 92, 93, 66, 228, 215, 65, 206, 170, 247, 128, 108, 64, 208, 95, 240, 50, 48, 167, 66, 143, 197, 246, 53, 91, 235, 26, 55, 168, 60, 60, 82, 225, 63, 199, 26, 123, 197, 212, 12, 146, 226, 40, 184, 158, 26, 37, 85, 246, 86, 105, 58, 210, 67, 222, 238, 127 },
                            PasswordSalt = new byte[] { 215, 47, 18, 113, 52, 229, 91, 61, 0, 238, 253, 28, 92, 111, 251, 60, 68, 201, 66, 21, 190, 220, 170, 201, 128, 39, 192, 167, 126, 224, 239, 158, 212, 217, 179, 89, 181, 1, 166, 135, 255, 198, 143, 58, 19, 108, 181, 233, 162, 148, 211, 61, 125, 128, 26, 63, 102, 235, 188, 39, 158, 182, 60, 217, 184, 2, 214, 142, 40, 1, 45, 151, 129, 101, 140, 254, 61, 12, 74, 202, 93, 250, 95, 201, 20, 44, 63, 112, 176, 160, 124, 107, 138, 64, 231, 140, 171, 27, 216, 231, 81, 178, 234, 195, 118, 55, 167, 42, 149, 50, 18, 23, 39, 6, 93, 194, 68, 236, 127, 99, 191, 148, 93, 101, 220, 211, 48, 75 }
                        });
                });

            modelBuilder.Entity("DataManagerAPI.Repository.Abstractions.Models.UserData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Data")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

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
