using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddUseOneMinuteIntervalsFromToAssignedSite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UseOneMinuteIntervalsFrom",
                table: "AssignedSiteVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UseOneMinuteIntervalsFrom",
                table: "AssignedSites",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseOneMinuteIntervalsFrom",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "UseOneMinuteIntervalsFrom",
                table: "AssignedSites");
        }
    }
}
