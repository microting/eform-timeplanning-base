using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    public partial class AddingWorkerComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WorkerComment",
                table: "PlanRegistrationVersions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "WorkerComment",
                table: "PlanRegistrations",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkerComment",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "WorkerComment",
                table: "PlanRegistrations");
        }
    }
}
