using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TableMgmtApp.Migrations
{
    /// <inheritdoc />
    public partial class fulldiscountplayerrelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Discounts_DiscountId",
                table: "Players");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Discounts_DiscountId",
                table: "Players",
                column: "DiscountId",
                principalTable: "Discounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Discounts_DiscountId",
                table: "Players");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Discounts_DiscountId",
                table: "Players",
                column: "DiscountId",
                principalTable: "Discounts",
                principalColumn: "Id");
        }
    }
}
