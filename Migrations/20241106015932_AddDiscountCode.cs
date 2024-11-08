using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KhielsSkincare.Migrations
{
    public partial class AddDiscountCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "Discount");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Discount",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Discount");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "Discount",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
