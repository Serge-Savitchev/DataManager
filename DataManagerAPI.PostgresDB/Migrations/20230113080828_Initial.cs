using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataManagerAPI.PostgresDB.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    LastName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    OwnerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_Role",
                        column: x => x.Role,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCredentials",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Login = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "bytea", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "bytea", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCredentials", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserCredentials_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Data = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserData_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "PowerUser" },
                    { 3, "User" },
                    { 4, "ReadOnlyUser" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "OwnerId", "Role" },
                values: new object[] { 1, null, "DefaultAdmin", "DefaultAdmin", 0, 1 });

            migrationBuilder.InsertData(
                table: "UserCredentials",
                columns: new[] { "UserId", "Login", "PasswordHash", "PasswordSalt", "RefreshToken" },
                values: new object[] { 1, "Admin", new byte[] { 89, 201, 15, 23, 149, 5, 10, 74, 92, 93, 66, 228, 215, 65, 206, 170, 247, 128, 108, 64, 208, 95, 240, 50, 48, 167, 66, 143, 197, 246, 53, 91, 235, 26, 55, 168, 60, 60, 82, 225, 63, 199, 26, 123, 197, 212, 12, 146, 226, 40, 184, 158, 26, 37, 85, 246, 86, 105, 58, 210, 67, 222, 238, 127 }, new byte[] { 215, 47, 18, 113, 52, 229, 91, 61, 0, 238, 253, 28, 92, 111, 251, 60, 68, 201, 66, 21, 190, 220, 170, 201, 128, 39, 192, 167, 126, 224, 239, 158, 212, 217, 179, 89, 181, 1, 166, 135, 255, 198, 143, 58, 19, 108, 181, 233, 162, 148, 211, 61, 125, 128, 26, 63, 102, 235, 188, 39, 158, 182, 60, 217, 184, 2, 214, 142, 40, 1, 45, 151, 129, 101, 140, 254, 61, 12, 74, 202, 93, 250, 95, 201, 20, 44, 63, 112, 176, 160, 124, 107, 138, 64, 231, 140, 171, 27, 216, 231, 81, 178, 234, 195, 118, 55, 167, 42, 149, 50, 18, 23, 39, 6, 93, 194, 68, 236, 127, 99, 191, 148, 93, 101, 220, 211, 48, 75 }, null });

            migrationBuilder.CreateIndex(
                name: "IX_UserData_UserId",
                table: "UserData",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Role",
                table: "Users",
                column: "Role");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCredentials");

            migrationBuilder.DropTable(
                name: "UserData");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
