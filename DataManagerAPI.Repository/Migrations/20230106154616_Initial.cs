using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataManagerAPI.Repository.Migrations
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
                values: new object[] { 1, "Admin", new byte[] { 181, 242, 203, 31, 89, 32, 163, 163, 215, 148, 146, 76, 140, 246, 30, 164, 26, 189, 3, 69, 33, 255, 226, 244, 18, 137, 54, 83, 28, 94, 184, 28, 51, 75, 141, 239, 228, 147, 107, 173, 101, 135, 69, 9, 253, 159, 220, 206, 29, 158, 14, 42, 223, 129, 237, 209, 203, 39, 210, 244, 64, 173, 221, 135 }, new byte[] { 96, 229, 128, 135, 135, 148, 52, 90, 36, 229, 179, 18, 72, 84, 192, 78, 174, 105, 194, 53, 93, 105, 19, 103, 29, 178, 70, 60, 21, 47, 142, 200, 55, 5, 232, 114, 222, 193, 19, 222, 178, 131, 249, 106, 170, 225, 132, 94, 73, 191, 219, 70, 196, 152, 219, 112, 196, 35, 43, 12, 33, 245, 42, 89, 66, 98, 60, 67, 184, 121, 23, 17, 107, 70, 93, 190, 17, 110, 165, 126, 235, 182, 24, 225, 252, 250, 229, 154, 73, 140, 175, 192, 86, 116, 79, 84, 133, 38, 210, 124, 249, 81, 152, 156, 160, 145, 22, 160, 189, 122, 25, 166, 6, 75, 95, 55, 161, 12, 212, 82, 13, 159, 244, 254, 112, 191, 169, 19 }, null });

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
