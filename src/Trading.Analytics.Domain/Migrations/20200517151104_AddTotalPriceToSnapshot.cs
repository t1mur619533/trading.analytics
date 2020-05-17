using Microsoft.EntityFrameworkCore.Migrations;
// ReSharper disable All

namespace Trading.Analytics.Domain.Migrations
{
    public partial class AddTotalPriceToSnapshot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PositionSnapshots_Positions_PositionId",
                table: "PositionSnapshots");

            migrationBuilder.DropColumn(
                name: "PortfolioId",
                table: "PositionSnapshots");

            migrationBuilder.AlterColumn<int>(
                name: "PositionId",
                table: "PositionSnapshots",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TotalPriceRub",
                table: "PortfolioSnapshots",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddForeignKey(
                name: "FK_PositionSnapshots_Positions_PositionId",
                table: "PositionSnapshots",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PositionSnapshots_Positions_PositionId",
                table: "PositionSnapshots");

            migrationBuilder.DropColumn(
                name: "TotalPriceRub",
                table: "PortfolioSnapshots");

            migrationBuilder.AlterColumn<int>(
                name: "PositionId",
                table: "PositionSnapshots",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "PortfolioId",
                table: "PositionSnapshots",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_PositionSnapshots_Positions_PositionId",
                table: "PositionSnapshots",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
