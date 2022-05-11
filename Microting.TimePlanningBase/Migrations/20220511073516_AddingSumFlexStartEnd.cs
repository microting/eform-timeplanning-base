using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    public partial class AddingSumFlexStartEnd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SumFlex",
                table: "PlanRegistrationVersions",
                newName: "SumFlexStart");

            migrationBuilder.RenameColumn(
                name: "SumFlex",
                table: "PlanRegistrations",
                newName: "SumFlexStart");

            migrationBuilder.AddColumn<double>(
                name: "SumFlexEnd",
                table: "PlanRegistrationVersions",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SumFlexEnd",
                table: "PlanRegistrations",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SumFlexEnd",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "SumFlexEnd",
                table: "PlanRegistrations");

            migrationBuilder.RenameColumn(
                name: "SumFlexStart",
                table: "PlanRegistrationVersions",
                newName: "SumFlex");

            migrationBuilder.RenameColumn(
                name: "SumFlexStart",
                table: "PlanRegistrations",
                newName: "SumFlex");
        }
    }
}
