using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataManagerAPI.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Role = table.Column<int>(type: "int", nullable: false),
                    OwnerId = table.Column<int>(type: "int", nullable: false)
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
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Login = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                values: new object[] { 1, "Admin", new byte[] { 49, 191, 184, 224, 241, 149, 205, 95, 146, 83, 239, 199, 84, 146, 223, 210, 40, 136, 241, 150, 196, 189, 177, 95, 85, 25, 163, 222, 158, 99, 158, 31, 202, 109, 83, 26, 123, 92, 177, 52, 1, 49, 111, 244, 79, 50, 1, 29, 112, 144, 8, 43, 112, 126, 182, 208, 185, 129, 18, 51, 163, 84, 193, 103 }, new byte[] { 129, 46, 63, 179, 96, 243, 7, 41, 91, 123, 82, 2, 47, 220, 87, 9, 178, 60, 113, 46, 244, 240, 73, 181, 95, 76, 17, 228, 108, 36, 176, 195, 9, 33, 196, 120, 233, 19, 215, 61, 39, 111, 30, 147, 128, 75, 75, 79, 225, 216, 148, 232, 154, 244, 205, 81, 36, 204, 154, 115, 251, 196, 213, 223, 164, 197, 108, 177, 246, 67, 1, 228, 20, 81, 43, 159, 138, 170, 141, 125, 122, 42, 12, 47, 21, 0, 70, 172, 15, 97, 49, 57, 37, 172, 126, 1, 126, 223, 164, 11, 71, 76, 162, 82, 208, 60, 81, 231, 45, 137, 41, 131, 8, 165, 199, 252, 101, 145, 254, 100, 245, 154, 214, 225, 108, 84, 100, 55 }, null });

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
