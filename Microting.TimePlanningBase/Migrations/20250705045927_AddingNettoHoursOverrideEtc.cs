using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddingNettoHoursOverrideEtc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NettoHoursOverride",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NettoHoursOverride",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DaysBackInTimeAllowedEditing",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "DaysBackInTimeAllowedEditingEnabled",
                table: "AssignedSiteVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DaysBackInTimeAllowedEditing",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "DaysBackInTimeAllowedEditingEnabled",
                table: "AssignedSites",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NettoHoursOverride",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "NettoHoursOverride",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "DaysBackInTimeAllowedEditing",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "DaysBackInTimeAllowedEditingEnabled",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "DaysBackInTimeAllowedEditing",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "DaysBackInTimeAllowedEditingEnabled",
                table: "AssignedSites");
        }
    }
}
