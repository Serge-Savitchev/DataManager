using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataManagerAPI.SQLServerDB.Migrations
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
                values: new object[] { 1, "Admin", new byte[] { 183, 174, 122, 150, 10, 82, 208, 212, 77, 72, 34, 28, 218, 138, 100, 107, 118, 55, 51, 51, 224, 157, 123, 125, 234, 198, 42, 155, 45, 240, 18, 166, 140, 197, 98, 174, 216, 118, 101, 29, 131, 16, 54, 173, 50, 242, 41, 71, 58, 8, 89, 124, 219, 67, 208, 158, 188, 11, 190, 69, 116, 20, 130, 110 }, new byte[] { 117, 213, 235, 70, 251, 171, 14, 17, 97, 6, 13, 19, 47, 132, 7, 102, 199, 17, 99, 220, 98, 141, 197, 205, 45, 0, 5, 152, 199, 224, 173, 7, 222, 193, 33, 61, 79, 128, 72, 250, 131, 169, 35, 72, 114, 26, 79, 66, 143, 13, 133, 90, 63, 127, 0, 165, 214, 35, 83, 171, 19, 159, 194, 50, 27, 232, 127, 165, 133, 18, 34, 9, 187, 154, 228, 30, 163, 3, 84, 11, 112, 188, 229, 192, 217, 11, 17, 212, 253, 61, 140, 103, 203, 211, 182, 29, 18, 191, 32, 161, 92, 8, 70, 61, 158, 126, 204, 81, 32, 72, 143, 78, 82, 117, 56, 56, 221, 148, 182, 50, 28, 6, 198, 94, 27, 201, 192, 7 }, null });

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
