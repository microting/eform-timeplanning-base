using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddFlexChainComputedThroughToAssignedSite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FlexChainComputedThrough",
                table: "AssignedSiteVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FlexChainComputedThrough",
                table: "AssignedSites",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FlexChainComputedThrough",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "FlexChainComputedThrough",
                table: "AssignedSites");
        }
    }
}
