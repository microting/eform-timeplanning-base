using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddPayrollCodeAndIntegrationSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add PayrollCode to PayTierRules + version
            migrationBuilder.AddColumn<string>(
                name: "PayrollCode",
                table: "PayTierRules",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayrollCode",
                table: "PayTierRuleVersions",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true);

            // Add PayrollCode to PayTimeBandRules + version
            migrationBuilder.AddColumn<string>(
                name: "PayrollCode",
                table: "PayTimeBandRules",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayrollCode",
                table: "PayTimeBandRuleVersions",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true);

            // Add PayrollCode to PlanRegistrationPayLines + version
            migrationBuilder.AddColumn<string>(
                name: "PayrollCode",
                table: "PlanRegistrationPayLines",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayrollCode",
                table: "PlanRegistrationPayLineVersions",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true);

            // Create PayrollIntegrationSettings table
            migrationBuilder.CreateTable(
                name: "PayrollIntegrationSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PayrollSystem = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CutoffDay = table.Column<int>(type: "int", nullable: false, defaultValue: 19),
                    ApiBaseUrl = table.Column<string>(type: "longtext", nullable: true),
                    ApiKey = table.Column<string>(type: "longtext", nullable: true),
                    ApiSecret = table.Column<string>(type: "longtext", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    WorkflowState = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollIntegrationSettings", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Create PayrollIntegrationSettingsVersions table
            migrationBuilder.CreateTable(
                name: "PayrollIntegrationSettingsVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PayrollIntegrationSettingsId = table.Column<int>(type: "int", nullable: false),
                    PayrollSystem = table.Column<int>(type: "int", nullable: false),
                    CutoffDay = table.Column<int>(type: "int", nullable: false),
                    ApiBaseUrl = table.Column<string>(type: "longtext", nullable: true),
                    ApiKey = table.Column<string>(type: "longtext", nullable: true),
                    ApiSecret = table.Column<string>(type: "longtext", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    WorkflowState = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollIntegrationSettingsVersions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "PayrollIntegrationSettingsVersions");
            migrationBuilder.DropTable(name: "PayrollIntegrationSettings");

            migrationBuilder.DropColumn(name: "PayrollCode", table: "PlanRegistrationPayLineVersions");
            migrationBuilder.DropColumn(name: "PayrollCode", table: "PlanRegistrationPayLines");
            migrationBuilder.DropColumn(name: "PayrollCode", table: "PayTimeBandRuleVersions");
            migrationBuilder.DropColumn(name: "PayrollCode", table: "PayTimeBandRules");
            migrationBuilder.DropColumn(name: "PayrollCode", table: "PayTierRuleVersions");
            migrationBuilder.DropColumn(name: "PayrollCode", table: "PayTierRules");
        }
    }
}
