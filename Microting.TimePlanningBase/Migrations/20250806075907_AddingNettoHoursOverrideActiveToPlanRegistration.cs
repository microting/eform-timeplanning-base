using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddingNettoHoursOverrideActiveToPlanRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NettoHoursOverrideActive",
                table: "PlanRegistrationVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NettoHoursOverrideActive",
                table: "PlanRegistrations",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NettoHoursOverrideActive",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "NettoHoursOverrideActive",
                table: "PlanRegistrations");
        }
    }
}
