using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TableMgmtApp.Migrations
{
    /// <inheritdoc />
    public partial class PlaySessionHasTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TableName",
                table: "PlaySessions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TableName",
                table: "PlaySessions");
        }
    }
}
