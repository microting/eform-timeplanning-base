using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddingExtraOvertimeHoursToPlanRegistrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ExtraOvertimeHours",
                table: "PlanRegistrationVersions",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExtraOvertimeHoursInSeconds",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ExtraOvertimeHours",
                table: "PlanRegistrations",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExtraOvertimeHoursInSeconds",
                table: "PlanRegistrations",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtraOvertimeHours",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "ExtraOvertimeHoursInSeconds",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "ExtraOvertimeHours",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "ExtraOvertimeHoursInSeconds",
                table: "PlanRegistrations");
        }
    }
}
