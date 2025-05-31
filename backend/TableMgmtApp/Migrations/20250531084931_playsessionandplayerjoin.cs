using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TableMgmtApp.Migrations
{
    /// <inheritdoc />
    public partial class playsessionandplayerjoin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Player",
                table: "PlaySessions");

            migrationBuilder.AlterColumn<Guid>(
                name: "PlayerId",
                table: "PlaySessions",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_PlaySessions_PlayerId",
                table: "PlaySessions",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlaySessions_Players_PlayerId",
                table: "PlaySessions",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlaySessions_Players_PlayerId",
                table: "PlaySessions");

            migrationBuilder.DropIndex(
                name: "IX_PlaySessions_PlayerId",
                table: "PlaySessions");

            migrationBuilder.AlterColumn<Guid>(
                name: "PlayerId",
                table: "PlaySessions",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Player",
                table: "PlaySessions",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
