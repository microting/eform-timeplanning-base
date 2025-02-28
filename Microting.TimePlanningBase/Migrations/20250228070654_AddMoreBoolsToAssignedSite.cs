using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreBoolsToAssignedSite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UseGoogleSheetAsDefault",
                table: "AssignedSiteVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "UseOnlyPlanHours",
                table: "AssignedSiteVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseGoogleSheetAsDefault",
                table: "AssignedSites",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseOnlyPlanHours",
                table: "AssignedSites",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseGoogleSheetAsDefault",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "UseOnlyPlanHours",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "UseGoogleSheetAsDefault",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "UseOnlyPlanHours",
                table: "AssignedSites");
        }
    }
}
