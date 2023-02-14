﻿// <auto-generated />
using DataManagerAPI.PostgresDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataManagerAPI.PostgresDB.Migrations
{
    [DbContext(typeof(PostgresDBContext))]
    partial class PostgresDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
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

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("text");

                    b.HasKey("UserId");

                    b.ToTable("UserCredentials");

                    b.HasData(
                        new
                        {
                            UserId = 1,
                            Login = "Admin",
                            PasswordHash = "$2a$11$Jtrvecdcjaa7dZ0KWuR6/.vv3rI0RvIquelLLRu2yEn0eYU2Td5pi"
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

            modelBuilder.Entity("DataManagerAPI.Repository.Abstractions.Models.UserFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<long>("Size")
                        .HasColumnType("bigint");

                    b.Property<int>("UserDataId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserDataId");

                    b.ToTable("UserFiles");
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

            modelBuilder.Entity("DataManagerAPI.Repository.Abstractions.Models.UserFile", b =>
                {
                    b.HasOne("DataManagerAPI.Repository.Abstractions.Models.UserData", null)
                        .WithMany("UserFiles")
                        .HasForeignKey("UserDataId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataManagerAPI.Repository.Abstractions.Models.UserData", b =>
                {
                    b.Navigation("UserFiles");
                });
#pragma warning restore 612, 618
        }
    }
}
