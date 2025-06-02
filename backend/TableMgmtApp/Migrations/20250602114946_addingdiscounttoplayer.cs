using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TableMgmtApp.Migrations
{
    /// <inheritdoc />
    public partial class addingdiscounttoplayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Players_DiscountId",
                table: "Players",
                column: "DiscountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Discounts_DiscountId",
                table: "Players",
                column: "DiscountId",
                principalTable: "Discounts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Discounts_DiscountId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_DiscountId",
                table: "Players");
        }
    }
}
