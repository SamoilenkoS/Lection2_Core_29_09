using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lection2_Core_DAL.Migrations
{
    public partial class RolesSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Title" },
                values: new object[] { new Guid("9d25f40b-88de-4e7f-b76b-74f87f26f654"), "Admin" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Title" },
                values: new object[] { new Guid("a2a9a6ba-cc43-4251-bfc9-34791264a417"), "User" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("9d25f40b-88de-4e7f-b76b-74f87f26f654"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("a2a9a6ba-cc43-4251-bfc9-34791264a417"));
        }
    }
}
