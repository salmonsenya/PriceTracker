using Microsoft.EntityFrameworkCore.Migrations;

namespace PriceTracker.Migrations
{
    public partial class PriceIntToFloat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Price",
                table: "Items",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Price",
                table: "Items",
                type: "int",
                nullable: true,
                oldClrType: typeof(float),
                oldNullable: true);
        }
    }
}
