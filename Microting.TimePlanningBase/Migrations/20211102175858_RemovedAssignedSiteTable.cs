using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microting.TimePlanningBase.Migrations
{
    public partial class RemovedAssignedSiteTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlanRegistrations_AssignedSites_AssignedSiteId",
                table: "PlanRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_PlanRegistrationVersions_AssignedSites_AssignedSiteId",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropTable(
                name: "AssignedSites");

            migrationBuilder.DropTable(
                name: "AssignedSiteVersions");

            migrationBuilder.DropIndex(
                name: "IX_PlanRegistrationVersions_AssignedSiteId",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropIndex(
                name: "IX_PlanRegistrations_AssignedSiteId",
                table: "PlanRegistrations");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssignedSites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    SiteId = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    WorkflowState = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignedSites", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AssignedSiteVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AssignedSiteId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    SiteId = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    WorkflowState = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignedSiteVersions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_PlanRegistrationVersions_AssignedSiteId",
                table: "PlanRegistrationVersions",
                column: "AssignedSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanRegistrations_AssignedSiteId",
                table: "PlanRegistrations",
                column: "AssignedSiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlanRegistrations_AssignedSites_AssignedSiteId",
                table: "PlanRegistrations",
                column: "AssignedSiteId",
                principalTable: "AssignedSites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlanRegistrationVersions_AssignedSites_AssignedSiteId",
                table: "PlanRegistrationVersions",
                column: "AssignedSiteId",
                principalTable: "AssignedSites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
