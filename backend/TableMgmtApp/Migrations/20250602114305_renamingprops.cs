using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TableMgmtApp.Migrations
{
    /// <inheritdoc />
    public partial class renamingprops : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DiscountId",
                table: "Players",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountId",
                table: "Players");
        }
    }
}
