using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trading.Analytics.Domain.Migrations
{
    public partial class AddDateTimePortfolioSnapshot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateTime",
                table: "PositionSnapshots");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTime",
                table: "PortfolioSnapshots",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateTime",
                table: "PortfolioSnapshots");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTime",
                table: "PositionSnapshots",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
