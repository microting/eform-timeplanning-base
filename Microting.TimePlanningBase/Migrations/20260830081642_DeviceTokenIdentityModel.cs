using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class DeviceTokenIdentityModel : Migration
    {
        // Hand-written, and deliberately NOT the shape `dotnet ef migrations add`
        // scaffolds. Two properties matter here and neither is automatic:
        //
        // 1. DATA SAFETY. The scaffolded version dropped and re-added `Token`
        //    rather than renaming it, and added the new columns as NOT NULL,
        //    which destroys or rejects every existing row.
        //
        // 2. RETRY SAFETY. MariaDB auto-commits each DDL statement, so this
        //    migration is not atomic, and EF only writes __EFMigrationsHistory
        //    after the last statement. A failure partway therefore leaves the
        //    schema half-migrated with no history row, and the next
        //    Database.Migrate() at pod startup re-runs Up from the top. Every
        //    destructive or creative step below is consequently written to be
        //    a no-op when its effect is already present, so a re-run
        //    self-heals instead of dying on `ERROR 1091: Can't DROP ...`.
        //    Migrate() runs at startup across ~250 tenant databases, so a
        //    non-idempotent step is a fleet-wide outage rather than one
        //    contained failure.
        //
        // Idempotency uses MariaDB's own `IF [NOT] EXISTS` DDL clauses rather
        // than information_schema probes driven by user variables and PREPARE.
        // Both express the same guard, but `@`-prefixed user variables are
        // parsed as parameter placeholders by MySqlConnector unless
        // AllowUserVariables=true, which is not set on the connection strings
        // this library is used through. These clauses have been in MariaDB
        // since 10.0/10.1; this stack is MariaDB-only (CI pins mariadb:10.8,
        // production is MariaDB Galera).

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DROP INDEX IF EXISTS `IX_DeviceTokens_Token` ON `DeviceTokens`;");
            migrationBuilder.Sql(
                "DROP INDEX IF EXISTS `IX_DeviceTokens_SdkSiteId` ON `DeviceTokens`;");

            // Rename and widen in one statement. 512 matches the
            // BackendConfiguration table; FCM tokens are ~163 chars today.
            migrationBuilder.Sql(
                "ALTER TABLE `DeviceTokens` CHANGE COLUMN IF EXISTS " +
                "`Token` `FcmToken` varchar(512) CHARACTER SET utf8mb4 NULL;");
            migrationBuilder.Sql(
                "ALTER TABLE `DeviceTokenVersions` CHANGE COLUMN IF EXISTS " +
                "`Token` `FcmToken` longtext CHARACTER SET utf8mb4 NULL;");

            // Added NULL-able so existing rows survive; tightened after the
            // backfill below.
            //
            // AppId carries a column default only for the duration of this
            // migration. The deploy is a rolling one, so old pods keep serving
            // while this runs, and an old pod registering a device token here
            // would otherwise insert AppId = NULL and make the tightening fail.
            // The default is not permanent: the ALTER ... MODIFY that applies
            // NOT NULL below restates the column definition and so drops it.
            // InstallationId gets no default because every constant value
            // collides under the new unique index -- for that column the
            // re-runnability of this migration is the mitigation, not a default.
            migrationBuilder.Sql(
                "ALTER TABLE `DeviceTokens` ADD COLUMN IF NOT EXISTS " +
                "`AppId` varchar(32) CHARACTER SET utf8mb4 NULL DEFAULT 'time';");
            migrationBuilder.Sql(
                "ALTER TABLE `DeviceTokens` ADD COLUMN IF NOT EXISTS " +
                "`InstallationId` varchar(128) CHARACTER SET utf8mb4 NULL;");
            migrationBuilder.Sql(
                "ALTER TABLE `DeviceTokenVersions` ADD COLUMN IF NOT EXISTS " +
                "`AppId` longtext CHARACTER SET utf8mb4 NULL;");
            migrationBuilder.Sql(
                "ALTER TABLE `DeviceTokenVersions` ADD COLUMN IF NOT EXISTS " +
                "`InstallationId` longtext CHARACTER SET utf8mb4 NULL;");

            Backfill(migrationBuilder);

            // Second sweep, immediately before the constraint that a missed row
            // would break. Anything an old pod inserted while the statements
            // above were running is caught here.
            Backfill(migrationBuilder);

            // NOT NULL is what makes the unique index bite: MariaDB treats NULLs
            // as distinct, so a NULL-able column would let duplicate installs
            // through the index unnoticed.
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

            // The MODIFY above restates the column definition and so drops the
            // temporary default as a side effect. Stated explicitly so the
            // default's removal is deterministic rather than incidental.
            migrationBuilder.Sql(
                "ALTER TABLE `DeviceTokens` ALTER COLUMN IF EXISTS `AppId` DROP DEFAULT;");

            migrationBuilder.Sql(
                "CREATE UNIQUE INDEX IF NOT EXISTS `IX_DeviceTokens_AppId_InstallationId` " +
                "ON `DeviceTokens` (`AppId`, `InstallationId`);");
            migrationBuilder.Sql(
                "CREATE INDEX IF NOT EXISTS `IX_DeviceTokens_AppId_SdkSiteId_WorkflowState` " +
                "ON `DeviceTokens` (`AppId`, `SdkSiteId`, `WorkflowState`);");
            migrationBuilder.Sql(
                "CREATE INDEX IF NOT EXISTS `IX_DeviceTokens_FcmToken` " +
                "ON `DeviceTokens` (`FcmToken`);");
        }

        /// <summary>
        /// Stamps every row that predates the identity model. Written to be
        /// re-runnable and to be safe to call more than once in a single Up:
        /// COALESCE leaves an already-stamped row alone, so only genuinely
        /// unstamped rows are touched.
        /// </summary>
        private static void Backfill(MigrationBuilder migrationBuilder)
        {
            // Every pre-existing row is flutter-time: TimePlanning serves
            // exactly one app.
            //
            // The synthetic InstallationId derives from the primary key, not
            // from a hash of the token. The column is about to gain a unique
            // index together with AppId, and only the PK is guaranteed
            // distinct -- two rows may legitimately carry the same token value,
            // and a NULL token would hash to a single shared value.
            migrationBuilder.Sql(
                "UPDATE `DeviceTokens` SET " +
                "`AppId` = COALESCE(`AppId`, 'time'), " +
                "`InstallationId` = COALESCE(`InstallationId`, CONCAT('legacy:', `Id`)) " +
                "WHERE `AppId` IS NULL OR `InstallationId` IS NULL;");

            // The version table keys off DeviceTokenId rather than its own Id,
            // so a version row carries the same synthetic install id as the row
            // it snapshots.
            migrationBuilder.Sql(
                "UPDATE `DeviceTokenVersions` SET " +
                "`AppId` = COALESCE(`AppId`, 'time'), " +
                "`InstallationId` = COALESCE(`InstallationId`, CONCAT('legacy:', `DeviceTokenId`)) " +
                "WHERE `AppId` IS NULL OR `InstallationId` IS NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Lossy by nature: the old table had no notion of an app or an
            // install, so rolling back merges every app's tokens back into one
            // undifferentiated set.
            //
            // Order matters. Recreating IX_DeviceTokens_Token can fail outright
            // with ERROR 1062 when two installs share an FCM token -- a state
            // the new model permits and the old one forbids. That risky step is
            // therefore done FIRST, while AppId and InstallationId are still
            // populated, so an operator hitting it can inspect the offending
            // installs and decide which to keep. Dropping the columns before it
            // would destroy exactly the evidence needed to recover.
            // Probe before destroying anything, so the operator gets a sentence
            // rather than an ERROR 1062 several statements later. NULL tokens
            // are excluded: a unique index never collides on them, but GROUP BY
            // would still group them together and report a false positive.
            migrationBuilder.Sql(
                "BEGIN NOT ATOMIC " +
                "IF EXISTS (SELECT 1 FROM `DeviceTokens` WHERE `FcmToken` IS NOT NULL " +
                "GROUP BY `FcmToken` HAVING COUNT(*) > 1) THEN " +
                "SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = " +
                "'Rollback blocked: duplicate FcmToken rows. Merge or remove them, then retry.'; " +
                "END IF; " +
                "END");

            migrationBuilder.Sql(
                "DROP INDEX IF EXISTS `IX_DeviceTokens_FcmToken` ON `DeviceTokens`;");
            migrationBuilder.Sql(
                "DROP INDEX IF EXISTS `IX_DeviceTokens_AppId_SdkSiteId_WorkflowState` ON `DeviceTokens`;");
            migrationBuilder.Sql(
                "DROP INDEX IF EXISTS `IX_DeviceTokens_AppId_InstallationId` ON `DeviceTokens`;");

            // Narrowing truncates any token longer than 255 chars. FCM tokens
            // are ~163 chars today, so this is safe in practice, but it is a
            // real rollback caveat.
            migrationBuilder.Sql(
                "ALTER TABLE `DeviceTokens` CHANGE COLUMN IF EXISTS " +
                "`FcmToken` `Token` varchar(255) CHARACTER SET utf8mb4 NULL;");
            migrationBuilder.Sql(
                "ALTER TABLE `DeviceTokenVersions` CHANGE COLUMN IF EXISTS " +
                "`FcmToken` `Token` longtext CHARACTER SET utf8mb4 NULL;");

            migrationBuilder.Sql(
                "CREATE UNIQUE INDEX IF NOT EXISTS `IX_DeviceTokens_Token` " +
                "ON `DeviceTokens` (`Token`);");
            migrationBuilder.Sql(
                "CREATE INDEX IF NOT EXISTS `IX_DeviceTokens_SdkSiteId` " +
                "ON `DeviceTokens` (`SdkSiteId`);");

            // Only now, with the old key proven reconstructible.
            migrationBuilder.Sql(
                "ALTER TABLE `DeviceTokens` DROP COLUMN IF EXISTS `InstallationId`;");
            migrationBuilder.Sql(
                "ALTER TABLE `DeviceTokens` DROP COLUMN IF EXISTS `AppId`;");
            migrationBuilder.Sql(
                "ALTER TABLE `DeviceTokenVersions` DROP COLUMN IF EXISTS `InstallationId`;");
            migrationBuilder.Sql(
                "ALTER TABLE `DeviceTokenVersions` DROP COLUMN IF EXISTS `AppId`;");
        }
    }
}
