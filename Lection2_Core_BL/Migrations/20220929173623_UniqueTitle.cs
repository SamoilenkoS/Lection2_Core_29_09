using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lection2_Core_BL.Migrations
{
    public partial class UniqueTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Goods_Title",
                table: "Goods",
                column: "Title",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Goods_Title",
                table: "Goods");
        }
    }
}
