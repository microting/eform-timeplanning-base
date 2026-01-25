using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddingFileNameAndExtensionToPictureSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileExtension",
                table: "PictureSnapshotVersions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "PictureSnapshotVersions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FileExtension",
                table: "PictureSnapshots",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "PictureSnapshots",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileExtension",
                table: "PictureSnapshotVersions");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "PictureSnapshotVersions");

            migrationBuilder.DropColumn(
                name: "FileExtension",
                table: "PictureSnapshots");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "PictureSnapshots");
        }
    }
}
