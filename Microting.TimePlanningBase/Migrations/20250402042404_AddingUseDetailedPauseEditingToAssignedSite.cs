using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddingUseDetailedPauseEditingToAssignedSite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PlanChangedByAdmin",
                table: "PlanRegistrationVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PlanChangedByAdmin",
                table: "PlanRegistrations",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseDetailedPauseEditing",
                table: "AssignedSiteVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseDetailedPauseEditing",
                table: "AssignedSites",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlanChangedByAdmin",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "PlanChangedByAdmin",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "UseDetailedPauseEditing",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "UseDetailedPauseEditing",
                table: "AssignedSites");
        }
    }
}
