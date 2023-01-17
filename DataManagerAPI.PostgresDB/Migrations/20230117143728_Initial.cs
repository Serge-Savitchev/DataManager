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
                values: new object[] { 1, "Admin", new byte[] { 113, 18, 160, 168, 156, 69, 127, 148, 232, 102, 110, 0, 145, 34, 176, 132, 254, 113, 183, 72, 226, 31, 61, 18, 134, 225, 12, 22, 132, 209, 76, 251, 173, 214, 119, 65, 149, 98, 119, 77, 196, 78, 14, 245, 148, 79, 128, 251, 204, 146, 32, 28, 39, 229, 173, 19, 108, 70, 129, 252, 35, 10, 20, 54 }, new byte[] { 228, 174, 24, 147, 218, 214, 207, 62, 33, 8, 178, 138, 138, 101, 183, 50, 254, 54, 162, 44, 242, 176, 222, 172, 152, 37, 223, 21, 236, 13, 222, 202, 89, 209, 14, 86, 167, 132, 114, 150, 8, 34, 205, 76, 179, 114, 40, 254, 241, 12, 62, 248, 67, 51, 101, 242, 157, 157, 134, 240, 101, 75, 26, 233, 105, 239, 98, 204, 88, 6, 48, 221, 182, 134, 229, 250, 240, 4, 153, 118, 75, 88, 209, 174, 5, 16, 118, 115, 27, 215, 113, 191, 186, 195, 25, 235, 166, 131, 210, 145, 110, 151, 89, 82, 98, 157, 207, 246, 95, 32, 49, 11, 162, 188, 235, 189, 154, 162, 1, 144, 82, 146, 235, 53, 81, 175, 235, 177 }, null });

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
