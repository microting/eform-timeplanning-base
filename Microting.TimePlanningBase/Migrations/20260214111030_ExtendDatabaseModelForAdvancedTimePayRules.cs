using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class ExtendDatabaseModelForAdvancedTimePayRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CountHolidayPaidOffAsWork",
                table: "WorkingTimeRuleSetVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CountPaidAbsenceAsWork",
                table: "WorkingTimeRuleSetVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MonthlyNormMode",
                table: "WorkingTimeRuleSetVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OvertimeAllocationStrategy",
                table: "WorkingTimeRuleSetVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OvertimeAveragingWindowDays",
                table: "WorkingTimeRuleSetVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OvertimePeriodLengthDays",
                table: "WorkingTimeRuleSetVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CountHolidayPaidOffAsWork",
                table: "WorkingTimeRuleSets",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CountPaidAbsenceAsWork",
                table: "WorkingTimeRuleSets",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MonthlyNormMode",
                table: "WorkingTimeRuleSets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OvertimeAllocationStrategy",
                table: "WorkingTimeRuleSets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OvertimeAveragingWindowDays",
                table: "WorkingTimeRuleSets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OvertimePeriodLengthDays",
                table: "WorkingTimeRuleSets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HolidayPaidOffFixedSeconds",
                table: "PayRuleSetVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HolidayPaidOffMode",
                table: "PayRuleSetVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "HolidayPaidOffPayCode",
                table: "PayRuleSetVersions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "HolidayPaidOffFixedSeconds",
                table: "PayRuleSets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HolidayPaidOffMode",
                table: "PayRuleSets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "HolidayPaidOffPayCode",
                table: "PayRuleSets",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AssignedSiteRuleSetAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AssignedSiteId = table.Column<int>(type: "int", nullable: false),
                    ValidFromDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ValidToDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    PayRuleSetId = table.Column<int>(type: "int", nullable: true),
                    WorkingTimeRuleSetId = table.Column<int>(type: "int", nullable: true),
                    BreakPolicyId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_AssignedSiteRuleSetAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignedSiteRuleSetAssignments_AssignedSites_AssignedSiteId",
                        column: x => x.AssignedSiteId,
                        principalTable: "AssignedSites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssignedSiteRuleSetAssignments_BreakPolicies_BreakPolicyId",
                        column: x => x.BreakPolicyId,
                        principalTable: "BreakPolicies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssignedSiteRuleSetAssignments_PayRuleSets_PayRuleSetId",
                        column: x => x.PayRuleSetId,
                        principalTable: "PayRuleSets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssignedSiteRuleSetAssignments_WorkingTimeRuleSets_WorkingTi~",
                        column: x => x.WorkingTimeRuleSetId,
                        principalTable: "WorkingTimeRuleSets",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AssignedSiteRuleSetAssignmentsVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AssignedSiteRuleSetAssignmentsId = table.Column<int>(type: "int", nullable: false),
                    AssignedSiteId = table.Column<int>(type: "int", nullable: false),
                    ValidFromDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ValidToDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    PayRuleSetId = table.Column<int>(type: "int", nullable: true),
                    WorkingTimeRuleSetId = table.Column<int>(type: "int", nullable: true),
                    BreakPolicyId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_AssignedSiteRuleSetAssignmentsVersions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PayDayTypeRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PayRuleSetId = table.Column<int>(type: "int", nullable: false),
                    DayType = table.Column<int>(type: "int", nullable: false),
                    DefaultPayCode = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Priority = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_PayDayTypeRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayDayTypeRules_PayRuleSets_PayRuleSetId",
                        column: x => x.PayRuleSetId,
                        principalTable: "PayRuleSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PayDayTypeRuleVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PayDayTypeRuleId = table.Column<int>(type: "int", nullable: false),
                    PayRuleSetId = table.Column<int>(type: "int", nullable: false),
                    DayType = table.Column<int>(type: "int", nullable: false),
                    DefaultPayCode = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Priority = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_PayDayTypeRuleVersions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PayTimeBandRuleVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PayTimeBandRuleId = table.Column<int>(type: "int", nullable: false),
                    PayDayTypeRuleId = table.Column<int>(type: "int", nullable: false),
                    StartSecondOfDay = table.Column<int>(type: "int", nullable: false),
                    EndSecondOfDay = table.Column<int>(type: "int", nullable: false),
                    PayCode = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Priority = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_PayTimeBandRuleVersions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PayTimeBandRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PayDayTypeRuleId = table.Column<int>(type: "int", nullable: false),
                    StartSecondOfDay = table.Column<int>(type: "int", nullable: false),
                    EndSecondOfDay = table.Column<int>(type: "int", nullable: false),
                    PayCode = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Priority = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_PayTimeBandRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayTimeBandRules_PayDayTypeRules_PayDayTypeRuleId",
                        column: x => x.PayDayTypeRuleId,
                        principalTable: "PayDayTypeRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AssignedSiteRuleSetAssignments_AssignedSiteId_ValidFromDate",
                table: "AssignedSiteRuleSetAssignments",
                columns: new[] { "AssignedSiteId", "ValidFromDate" });

            migrationBuilder.CreateIndex(
                name: "IX_AssignedSiteRuleSetAssignments_BreakPolicyId",
                table: "AssignedSiteRuleSetAssignments",
                column: "BreakPolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignedSiteRuleSetAssignments_PayRuleSetId",
                table: "AssignedSiteRuleSetAssignments",
                column: "PayRuleSetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignedSiteRuleSetAssignments_WorkingTimeRuleSetId",
                table: "AssignedSiteRuleSetAssignments",
                column: "WorkingTimeRuleSetId");

            migrationBuilder.CreateIndex(
                name: "IX_PayDayTypeRules_PayRuleSetId_DayType_Priority",
                table: "PayDayTypeRules",
                columns: new[] { "PayRuleSetId", "DayType", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_PayTimeBandRules_PayDayTypeRuleId_Priority",
                table: "PayTimeBandRules",
                columns: new[] { "PayDayTypeRuleId", "Priority" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignedSiteRuleSetAssignments");

            migrationBuilder.DropTable(
                name: "AssignedSiteRuleSetAssignmentsVersions");

            migrationBuilder.DropTable(
                name: "PayDayTypeRuleVersions");

            migrationBuilder.DropTable(
                name: "PayTimeBandRules");

            migrationBuilder.DropTable(
                name: "PayTimeBandRuleVersions");

            migrationBuilder.DropTable(
                name: "PayDayTypeRules");

            migrationBuilder.DropColumn(
                name: "CountHolidayPaidOffAsWork",
                table: "WorkingTimeRuleSetVersions");

            migrationBuilder.DropColumn(
                name: "CountPaidAbsenceAsWork",
                table: "WorkingTimeRuleSetVersions");

            migrationBuilder.DropColumn(
                name: "MonthlyNormMode",
                table: "WorkingTimeRuleSetVersions");

            migrationBuilder.DropColumn(
                name: "OvertimeAllocationStrategy",
                table: "WorkingTimeRuleSetVersions");

            migrationBuilder.DropColumn(
                name: "OvertimeAveragingWindowDays",
                table: "WorkingTimeRuleSetVersions");

            migrationBuilder.DropColumn(
                name: "OvertimePeriodLengthDays",
                table: "WorkingTimeRuleSetVersions");

            migrationBuilder.DropColumn(
                name: "CountHolidayPaidOffAsWork",
                table: "WorkingTimeRuleSets");

            migrationBuilder.DropColumn(
                name: "CountPaidAbsenceAsWork",
                table: "WorkingTimeRuleSets");

            migrationBuilder.DropColumn(
                name: "MonthlyNormMode",
                table: "WorkingTimeRuleSets");

            migrationBuilder.DropColumn(
                name: "OvertimeAllocationStrategy",
                table: "WorkingTimeRuleSets");

            migrationBuilder.DropColumn(
                name: "OvertimeAveragingWindowDays",
                table: "WorkingTimeRuleSets");

            migrationBuilder.DropColumn(
                name: "OvertimePeriodLengthDays",
                table: "WorkingTimeRuleSets");

            migrationBuilder.DropColumn(
                name: "HolidayPaidOffFixedSeconds",
                table: "PayRuleSetVersions");

            migrationBuilder.DropColumn(
                name: "HolidayPaidOffMode",
                table: "PayRuleSetVersions");

            migrationBuilder.DropColumn(
                name: "HolidayPaidOffPayCode",
                table: "PayRuleSetVersions");

            migrationBuilder.DropColumn(
                name: "HolidayPaidOffFixedSeconds",
                table: "PayRuleSets");

            migrationBuilder.DropColumn(
                name: "HolidayPaidOffMode",
                table: "PayRuleSets");

            migrationBuilder.DropColumn(
                name: "HolidayPaidOffPayCode",
                table: "PayRuleSets");
        }
    }
}
