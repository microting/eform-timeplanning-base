using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddRegisteredUnderOneMinuteIntervalsToPlanRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RegisteredUnderOneMinuteIntervals",
                table: "PlanRegistrationVersions",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RegisteredUnderOneMinuteIntervals",
                table: "PlanRegistrations",
                type: "tinyint(1)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegisteredUnderOneMinuteIntervals",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "RegisteredUnderOneMinuteIntervals",
                table: "PlanRegistrations");
        }
    }
}
