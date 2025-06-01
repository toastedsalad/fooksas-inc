using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TableMgmtApp.Migrations
{
    /// <inheritdoc />
    public partial class addingdiscounttosession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DiscountId",
                table: "PlaySessions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlaySessions_DiscountId",
                table: "PlaySessions",
                column: "DiscountId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlaySessions_Discounts_DiscountId",
                table: "PlaySessions",
                column: "DiscountId",
                principalTable: "Discounts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlaySessions_Discounts_DiscountId",
                table: "PlaySessions");

            migrationBuilder.DropIndex(
                name: "IX_PlaySessions_DiscountId",
                table: "PlaySessions");

            migrationBuilder.DropColumn(
                name: "DiscountId",
                table: "PlaySessions");
        }
    }
}
