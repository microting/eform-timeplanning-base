using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddPauseOverrideMinutesToPlanRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Pause1OverrideMinutes",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Pause2OverrideMinutes",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Pause3OverrideMinutes",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Pause4OverrideMinutes",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Pause5OverrideMinutes",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Pause1OverrideMinutes",
                table: "PlanRegistrations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Pause2OverrideMinutes",
                table: "PlanRegistrations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Pause3OverrideMinutes",
                table: "PlanRegistrations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Pause4OverrideMinutes",
                table: "PlanRegistrations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Pause5OverrideMinutes",
                table: "PlanRegistrations",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pause1OverrideMinutes",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause2OverrideMinutes",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause3OverrideMinutes",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause4OverrideMinutes",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause5OverrideMinutes",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause1OverrideMinutes",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause2OverrideMinutes",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause3OverrideMinutes",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause4OverrideMinutes",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause5OverrideMinutes",
                table: "PlanRegistrations");
        }
    }
}
