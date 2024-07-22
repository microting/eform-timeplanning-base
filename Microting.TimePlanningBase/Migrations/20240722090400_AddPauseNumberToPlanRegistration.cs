using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddPauseNumberToPlanRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Shift1PauseNumber",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Shift2PauseNumber",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Shift1PauseNumber",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Shift2PauseNumber",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Shift1PauseNumber",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Shift2PauseNumber",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Shift1PauseNumber",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Shift2PauseNumber",
                table: "PlanRegistrations");
        }
    }
}
