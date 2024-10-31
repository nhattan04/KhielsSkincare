using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KhielsSkincare.Migrations
{
    public partial class Checkout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrderCode",
                table: "Shippings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderCode",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderCode",
                table: "Shippings");

            migrationBuilder.DropColumn(
                name: "OrderCode",
                table: "Payments");
        }
    }
}
