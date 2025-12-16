using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddGpsAndSnapshotEnabledToAssignedSite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "GpsEnabled",
                table: "AssignedSiteVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SnapshotEnabled",
                table: "AssignedSiteVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "GpsEnabled",
                table: "AssignedSites",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SnapshotEnabled",
                table: "AssignedSites",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GpsEnabled",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "SnapshotEnabled",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "GpsEnabled",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "SnapshotEnabled",
                table: "AssignedSites");
        }
    }
}
