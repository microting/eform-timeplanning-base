using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class MakeAssignedSiteResignedAtDateNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ResignedAtDate",
                table: "AssignedSiteVersions",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ResignedAtDate",
                table: "AssignedSites",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            // A live site that is not resigned has no resignation date. The old
            // NOT NULL column forced a placeholder (the web dialog wrote "today"
            // on every save), so clear it. AssignedSiteVersions is history and is
            // left exactly as it was written.
            migrationBuilder.Sql(
                "UPDATE AssignedSites SET ResignedAtDate = NULL WHERE Resigned = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // The NOT NULL column needs a value on every row before it can be
            // restored. Use default(DateTime) (0001-01-01), the value the old
            // non-nullable property held whenever nothing set it. Nothing reads
            // ResignedAtDate for a non-resigned site, which is the only kind of
            // row Up() leaves NULL, so the placeholder carries no meaning.
            migrationBuilder.Sql(
                "UPDATE AssignedSiteVersions SET ResignedAtDate = '0001-01-01 00:00:00' WHERE ResignedAtDate IS NULL");
            migrationBuilder.Sql(
                "UPDATE AssignedSites SET ResignedAtDate = '0001-01-01 00:00:00' WHERE ResignedAtDate IS NULL");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ResignedAtDate",
                table: "AssignedSiteVersions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ResignedAtDate",
                table: "AssignedSites",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);
        }
    }
}
