﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TableMgmtApp.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduleWeeklyRates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    WeeklyRates = table.Column<string>(type: "TEXT", nullable: false),
                    DefaultRate = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Schedules");
        }
    }
}
