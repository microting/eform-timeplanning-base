using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class DeviceTokenIdentityModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Hand-written. The scaffolded version dropped and re-added `Token`
            // instead of renaming it, and added the new columns as NOT NULL,
            // both of which destroy or reject existing rows. Order below is:
            // drop indexes -> rename -> widen -> add nullable -> backfill ->
            // tighten -> re-index.
            migrationBuilder.DropIndex(
                name: "IX_DeviceTokens_Token",
                table: "DeviceTokens");

            migrationBuilder.DropIndex(
                name: "IX_DeviceTokens_SdkSiteId",
                table: "DeviceTokens");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "DeviceTokens",
                newName: "FcmToken");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "DeviceTokenVersions",
                newName: "FcmToken");

            // Widen for parity with the BackendConfiguration table.
            migrationBuilder.AlterColumn<string>(
                name: "FcmToken",
                table: "DeviceTokens",
                type: "varchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            // Nullable first so existing rows survive the add.
            migrationBuilder.AddColumn<string>(
                name: "AppId",
                table: "DeviceTokens",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "InstallationId",
                table: "DeviceTokens",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "AppId",
                table: "DeviceTokenVersions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "InstallationId",
                table: "DeviceTokenVersions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            // Every pre-existing row in this table is flutter-time: TimePlanning
            // serves exactly one app.
            //
            // The synthetic InstallationId is derived from the primary key, not
            // from a hash of the token. The column is about to gain a unique
            // index together with AppId, and only the PK guarantees uniqueness;
            // two rows can legitimately share a token value. The version table
            // keys off DeviceTokenId rather than its own Id so a version row
            // carries the same synthetic id as the row it snapshots.
            migrationBuilder.Sql(
                "UPDATE `DeviceTokens` SET `AppId` = 'time', " +
                "`InstallationId` = CONCAT('legacy:', `Id`) " +
                "WHERE `AppId` IS NULL;");
            migrationBuilder.Sql(
                "UPDATE `DeviceTokenVersions` SET `AppId` = 'time', " +
                "`InstallationId` = CONCAT('legacy:', `DeviceTokenId`) " +
                "WHERE `AppId` IS NULL;");

            // Now safe to tighten. NOT NULL is what makes the unique index bite:
            // MySQL treats NULLs as distinct, so a nullable column would let
            // duplicate installs through.
            migrationBuilder.AlterColumn<string>(
                name: "AppId",
                table: "DeviceTokens",
                type: "varchar(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "InstallationId",
                table: "DeviceTokens",
                type: "varchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldMaxLength: 128,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceTokens_AppId_InstallationId",
                table: "DeviceTokens",
                columns: new[] { "AppId", "InstallationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceTokens_AppId_SdkSiteId_WorkflowState",
                table: "DeviceTokens",
                columns: new[] { "AppId", "SdkSiteId", "WorkflowState" });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceTokens_FcmToken",
                table: "DeviceTokens",
                column: "FcmToken");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Lossy by nature: the pre-migration table had no notion of an app
            // or an install, so rolling back merges every app's tokens back into
            // one undifferentiated set. Recreating IX_DeviceTokens_Token can
            // also fail outright if two installs share an FCM token, which the
            // new model permits and the old one did not.
            migrationBuilder.DropIndex(
                name: "IX_DeviceTokens_FcmToken",
                table: "DeviceTokens");

            migrationBuilder.DropIndex(
                name: "IX_DeviceTokens_AppId_SdkSiteId_WorkflowState",
                table: "DeviceTokens");

            migrationBuilder.DropIndex(
                name: "IX_DeviceTokens_AppId_InstallationId",
                table: "DeviceTokens");

            migrationBuilder.DropColumn(
                name: "InstallationId",
                table: "DeviceTokens");

            migrationBuilder.DropColumn(
                name: "AppId",
                table: "DeviceTokens");

            migrationBuilder.DropColumn(
                name: "InstallationId",
                table: "DeviceTokenVersions");

            migrationBuilder.DropColumn(
                name: "AppId",
                table: "DeviceTokenVersions");

            // Truncates any token longer than 255 chars. FCM tokens are ~163
            // chars today, so this is safe in practice, but it is a real caveat.
            migrationBuilder.AlterColumn<string>(
                name: "FcmToken",
                table: "DeviceTokens",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(512)",
                oldMaxLength: 512,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.RenameColumn(
                name: "FcmToken",
                table: "DeviceTokens",
                newName: "Token");

            migrationBuilder.RenameColumn(
                name: "FcmToken",
                table: "DeviceTokenVersions",
                newName: "Token");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceTokens_SdkSiteId",
                table: "DeviceTokens",
                column: "SdkSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceTokens_Token",
                table: "DeviceTokens",
                column: "Token",
                unique: true);
        }
    }
}
