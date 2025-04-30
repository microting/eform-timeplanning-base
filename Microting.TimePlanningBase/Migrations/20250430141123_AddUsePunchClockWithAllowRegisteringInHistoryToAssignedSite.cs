using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddUsePunchClockWithAllowRegisteringInHistoryToAssignedSite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UsePunchClockWithAllowRegisteringInHistory",
                table: "AssignedSiteVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UsePunchClockWithAllowRegisteringInHistory",
                table: "AssignedSites",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsePunchClockWithAllowRegisteringInHistory",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "UsePunchClockWithAllowRegisteringInHistory",
                table: "AssignedSites");
        }
    }
}
