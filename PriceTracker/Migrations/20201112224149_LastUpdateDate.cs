using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PriceTracker.Migrations
{
    public partial class LastUpdateDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdateDate",
                table: "Items",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdateDate",
                table: "Items");
        }
    }
}
