using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TableMgmtApp.Migrations
{
    /// <inheritdoc />
    public partial class dbconfigfordiscounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlaySessions_Discounts_DiscountId",
                table: "PlaySessions");

            migrationBuilder.AddForeignKey(
                name: "FK_PlaySessions_Discounts_DiscountId",
                table: "PlaySessions",
                column: "DiscountId",
                principalTable: "Discounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlaySessions_Discounts_DiscountId",
                table: "PlaySessions");

            migrationBuilder.AddForeignKey(
                name: "FK_PlaySessions_Discounts_DiscountId",
                table: "PlaySessions",
                column: "DiscountId",
                principalTable: "Discounts",
                principalColumn: "Id");
        }
    }
}
