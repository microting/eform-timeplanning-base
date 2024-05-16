using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddingRegistrationDeviceIdToPlanRegistrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegistrationDeviceId",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegistrationDeviceId",
                table: "PlanRegistrations",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegistrationDeviceId",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "RegistrationDeviceId",
                table: "PlanRegistrations");
        }
    }
}
