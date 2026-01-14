using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddPayRuleModelsAndWorkingTimeRuleSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PayRuleSetId",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkingTimeRuleSetId",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PayRuleSetId",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkingTimeRuleSetId",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PayDayRuleVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PayDayRuleId = table.Column<int>(type: "int", nullable: false),
                    PayRuleSetId = table.Column<int>(type: "int", nullable: false),
                    DayCode = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    WorkflowState = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayDayRuleVersions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PayRuleSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    WorkflowState = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayRuleSets", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PayRuleSetVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PayRuleSetId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    WorkflowState = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayRuleSetVersions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PayTierRuleVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PayTierRuleId = table.Column<int>(type: "int", nullable: false),
                    PayDayRuleId = table.Column<int>(type: "int", nullable: false),
                    UpToSeconds = table.Column<int>(type: "int", nullable: true),
                    PayCode = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Order = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    WorkflowState = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayTierRuleVersions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlanRegistrationPayLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PlanRegistrationId = table.Column<int>(type: "int", nullable: false),
                    PayCode = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hours = table.Column<double>(type: "double", nullable: false),
                    HoursInSeconds = table.Column<int>(type: "int", nullable: false),
                    PayRuleSetId = table.Column<int>(type: "int", nullable: true),
                    CalculatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    WorkflowState = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanRegistrationPayLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanRegistrationPayLines_PlanRegistrations_PlanRegistrationId",
                        column: x => x.PlanRegistrationId,
                        principalTable: "PlanRegistrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlanRegistrationPayLineVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PlanRegistrationPayLineId = table.Column<int>(type: "int", nullable: false),
                    PlanRegistrationId = table.Column<int>(type: "int", nullable: false),
                    PayCode = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hours = table.Column<double>(type: "double", nullable: false),
                    HoursInSeconds = table.Column<int>(type: "int", nullable: false),
                    PayRuleSetId = table.Column<int>(type: "int", nullable: true),
                    CalculatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    WorkflowState = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanRegistrationPayLineVersions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WorkingTimeRuleSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WeeklyNormalSeconds = table.Column<int>(type: "int", nullable: false),
                    DailyNormalSeconds = table.Column<int>(type: "int", nullable: true),
                    MinimumDailyRestSeconds = table.Column<int>(type: "int", nullable: false),
                    MinimumWeeklyRestSeconds = table.Column<int>(type: "int", nullable: false),
                    WeekStartsOn = table.Column<int>(type: "int", nullable: false),
                    NightStartSeconds = table.Column<int>(type: "int", nullable: false),
                    NightEndSeconds = table.Column<int>(type: "int", nullable: false),
                    OvertimeBasis = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    WorkflowState = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkingTimeRuleSets", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WorkingTimeRuleSetVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    WorkingTimeRuleSetId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WeeklyNormalSeconds = table.Column<int>(type: "int", nullable: false),
                    DailyNormalSeconds = table.Column<int>(type: "int", nullable: true),
                    MinimumDailyRestSeconds = table.Column<int>(type: "int", nullable: false),
                    MinimumWeeklyRestSeconds = table.Column<int>(type: "int", nullable: false),
                    WeekStartsOn = table.Column<int>(type: "int", nullable: false),
                    NightStartSeconds = table.Column<int>(type: "int", nullable: false),
                    NightEndSeconds = table.Column<int>(type: "int", nullable: false),
                    OvertimeBasis = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    WorkflowState = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkingTimeRuleSetVersions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PayDayRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PayRuleSetId = table.Column<int>(type: "int", nullable: false),
                    DayCode = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    WorkflowState = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayDayRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayDayRules_PayRuleSets_PayRuleSetId",
                        column: x => x.PayRuleSetId,
                        principalTable: "PayRuleSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PayTierRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PayDayRuleId = table.Column<int>(type: "int", nullable: false),
                    UpToSeconds = table.Column<int>(type: "int", nullable: true),
                    PayCode = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Order = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    WorkflowState = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayTierRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayTierRules_PayDayRules_PayDayRuleId",
                        column: x => x.PayDayRuleId,
                        principalTable: "PayDayRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AssignedSites_PayRuleSetId",
                table: "AssignedSites",
                column: "PayRuleSetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignedSites_WorkingTimeRuleSetId",
                table: "AssignedSites",
                column: "WorkingTimeRuleSetId");

            migrationBuilder.CreateIndex(
                name: "IX_PayDayRules_PayRuleSetId_DayCode",
                table: "PayDayRules",
                columns: new[] { "PayRuleSetId", "DayCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PayTierRules_PayDayRuleId_Order",
                table: "PayTierRules",
                columns: new[] { "PayDayRuleId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_PlanRegistrationPayLines_PlanRegistrationId",
                table: "PlanRegistrationPayLines",
                column: "PlanRegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanRegistrationPayLines_PlanRegistrationId_PayCode",
                table: "PlanRegistrationPayLines",
                columns: new[] { "PlanRegistrationId", "PayCode" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedSites_PayRuleSets_PayRuleSetId",
                table: "AssignedSites",
                column: "PayRuleSetId",
                principalTable: "PayRuleSets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedSites_WorkingTimeRuleSets_WorkingTimeRuleSetId",
                table: "AssignedSites",
                column: "WorkingTimeRuleSetId",
                principalTable: "WorkingTimeRuleSets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignedSites_PayRuleSets_PayRuleSetId",
                table: "AssignedSites");

            migrationBuilder.DropForeignKey(
                name: "FK_AssignedSites_WorkingTimeRuleSets_WorkingTimeRuleSetId",
                table: "AssignedSites");

            migrationBuilder.DropTable(
                name: "PayDayRuleVersions");

            migrationBuilder.DropTable(
                name: "PayRuleSetVersions");

            migrationBuilder.DropTable(
                name: "PayTierRules");

            migrationBuilder.DropTable(
                name: "PayTierRuleVersions");

            migrationBuilder.DropTable(
                name: "PlanRegistrationPayLines");

            migrationBuilder.DropTable(
                name: "PlanRegistrationPayLineVersions");

            migrationBuilder.DropTable(
                name: "WorkingTimeRuleSets");

            migrationBuilder.DropTable(
                name: "WorkingTimeRuleSetVersions");

            migrationBuilder.DropTable(
                name: "PayDayRules");

            migrationBuilder.DropTable(
                name: "PayRuleSets");

            migrationBuilder.DropIndex(
                name: "IX_AssignedSites_PayRuleSetId",
                table: "AssignedSites");

            migrationBuilder.DropIndex(
                name: "IX_AssignedSites_WorkingTimeRuleSetId",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "PayRuleSetId",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "WorkingTimeRuleSetId",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "PayRuleSetId",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "WorkingTimeRuleSetId",
                table: "AssignedSites");
        }
    }
}
