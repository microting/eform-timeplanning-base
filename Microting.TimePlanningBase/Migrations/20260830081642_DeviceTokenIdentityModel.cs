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
        // Re-running is NOT unconditionally self-healing, and one case in
        // particular a re-run can never fix: a genuine duplicate
        // (AppId, InstallationId) fails the CREATE UNIQUE INDEX with ERROR 1062
        // identically on every pass, so that tenant crash-loops until someone
        // de-duplicates `DeviceTokens` by hand. It cannot originate in the
        // backfill below - 'legacy:<PK>' is unique by construction - only in
        // rows a client wrote during the deploy window.
        //
        // Idempotency uses MariaDB's own `IF [NOT] EXISTS` DDL clauses rather
        // than information_schema probes driven by user variables and PREPARE.
        // Both express the same guard, but `@`-prefixed user variables are
        // parsed as parameter placeholders by MySqlConnector unless
        // AllowUserVariables=true, which is not set on the connection strings
        // this library is used through. These clauses have been in MariaDB
        // since 10.0/10.1; this stack is MariaDB-only (CI pins mariadb:10.8,
        // production is MariaDB Galera).

        // Stamps every row that predates the identity model.
        //
        // Every pre-existing row is flutter-time: TimePlanning serves exactly
        // one app.
        //
        // The synthetic InstallationId derives from the primary key, not from a
        // hash of the token. The column is about to gain a unique index
        // together with AppId, and only the PK is guaranteed distinct -- two
        // rows may legitimately carry the same token value, and a NULL token
        // would hash to a single shared value.
        //
        // The IS NULL guard makes this re-enterable after a partially applied
        // migration: only rows that still need a value are touched. It is per
        // COLUMN rather than per row, because the DEFAULT on AppId means a row
        // can arrive with AppId already set and InstallationId still NULL.
        private const string BackfillDeviceTokens =
            "UPDATE `DeviceTokens` SET " +
            "`AppId` = COALESCE(`AppId`, 'time'), " +
            "`InstallationId` = COALESCE(`InstallationId`, CONCAT('legacy:', `Id`)) " +
            "WHERE `AppId` IS NULL OR `InstallationId` IS NULL;";

        // The version table keys off DeviceTokenId rather than its own Id, so a
        // version row carries the same synthetic install id as the row it
        // snapshots.
        private const string BackfillDeviceTokenVersions =
            "UPDATE `DeviceTokenVersions` SET " +
            "`AppId` = COALESCE(`AppId`, 'time'), " +
            "`InstallationId` = COALESCE(`InstallationId`, CONCAT('legacy:', `DeviceTokenId`)) " +
            "WHERE `AppId` IS NULL OR `InstallationId` IS NULL;";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DROP INDEX IF EXISTS `IX_DeviceTokens_Token` ON `DeviceTokens`;");
            migrationBuilder.Sql(
                "DROP INDEX IF EXISTS `IX_DeviceTokens_SdkSiteId` ON `DeviceTokens`;");

            // CHANGE COLUMN IF EXISTS, not RENAME COLUMN: the guard is on the
            // OLD name still being present, so a re-run after a completed
            // rename does nothing.
            //
            // Rename and widen in one statement. 512 matches the
            // BackendConfiguration table; FCM tokens are ~163 chars today. On
            // `DeviceTokens` the type is a genuine widen (255 -> 512); on
            // `DeviceTokenVersions` `longtext` is unchanged and is there only
            // because CHANGE COLUMN requires the full specification - that one
            // is a restatement, not a retype.
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
            //
            // That default protects the FIRST pass only. ADD COLUMN IF NOT
            // EXISTS matches on column NAME alone, so on any retry after a
            // failure at or beyond the AppId tightening below this add is a
            // no-op and does NOT restore DEFAULT 'time' - AppId is by then NOT
            // NULL with no default, and an old pod's INSERT that omits AppId
            // fails with ERROR 1364 for the whole retry window. That is loud
            // and client-retryable rather than silently wrong, which is the
            // acceptable trade here, but it is the opposite of what "the
            // default keeps old pods writing" would suggest on its own.
            //
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

            migrationBuilder.Sql(BackfillDeviceTokens);
            migrationBuilder.Sql(BackfillDeviceTokenVersions);

            // NOT NULL is what makes the unique index bite: MariaDB treats NULLs
            // as distinct, so a NULL-able column would let duplicate installs
            // through the index unnoticed.
            //
            // These two stay plain EF AlterColumn calls, deliberately
            // unguarded. EF emits them as `MODIFY COLUMN`, which restates the
            // column's whole definition, so applying it to an already-tightened
            // column is a no-op: MODIFY is self-idempotent. Every statement
            // above needed an explicit IF [NOT] EXISTS guard precisely because
            // DROP INDEX, CHANGE COLUMN and ADD COLUMN are not.
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

            // Defence in depth, not the mechanism: the MODIFY above is what
            // actually drops AppId's default, by restating the column without a
            // DEFAULT clause. This says it outright so the intent survives a
            // provider that one day emits a definition which preserves it. The
            // default existed only to keep the migration window safe; the model
            // has no default, and leaving one would silently stamp 'time' on
            // any future insert that forgot to set AppId.
            migrationBuilder.Sql(
                "ALTER TABLE `DeviceTokens` ALTER COLUMN IF EXISTS `AppId` DROP DEFAULT;");

            // Second sweep - the version table ONLY.
            //
            // The matching sweep of `DeviceTokens` would be dead code here:
            // both its columns are NOT NULL by this point, so
            // `WHERE AppId IS NULL OR InstallationId IS NULL` matches zero
            // rows, always. Nor could any re-ordering rescue a row an old pod
            // inserted while the tightenings ran - such a row carries a NULL
            // InstallationId and dies on the `MODIFY ... NOT NULL` above
            // (ERROR 1138 strict, 1265 non-strict), before control ever reaches
            // this line. That race is not closable by ordering: it is closed by
            // the migration failing loudly there, and by the guarded re-run
            // (see the header) sweeping the offending row on its next pass.
            //
            // `DeviceTokenVersions` is the different case: its
            // AppId/InstallationId stay nullable longtext, nothing ever tightens
            // them, so a version snapshot written in that same window really can
            // still be sitting here with NULLs. Hence this one statement, and
            // only this one.
            migrationBuilder.Sql(BackfillDeviceTokenVersions);

            migrationBuilder.Sql(
                "CREATE UNIQUE INDEX IF NOT EXISTS `IX_DeviceTokens_AppId_InstallationId` " +
                "ON `DeviceTokens` (`AppId`, `InstallationId`);");
            migrationBuilder.Sql(
                "CREATE INDEX IF NOT EXISTS `IX_DeviceTokens_AppId_SdkSiteId_WorkflowState` " +
                "ON `DeviceTokens` (`AppId`, `SdkSiteId`, `WorkflowState`);");
            migrationBuilder.Sql(
                "CREATE INDEX IF NOT EXISTS `IX_DeviceTokens_FcmToken` " +
                "ON `DeviceTokens` (`FcmToken`);");

            // Verify the identity constraint actually landed, because CREATE
            // INDEX IF NOT EXISTS matches on index NAME alone. A tenant that
            // somehow already carries an index called
            // IX_DeviceTokens_AppId_InstallationId which is non-unique, or on
            // different columns, gets Note 1061 and the CREATE above is skipped
            // - the entire point of this migration would then be silently
            // absent. No migration in this history creates that name, so the
            // probability is low; the failure mode is silent, which is what
            // makes it worth a statement.
            //
            // Counting information_schema.STATISTICS rows restricted to the two
            // expected column names with NON_UNIQUE = 0 catches all three ways
            // it can be wrong: not unique, too few columns, wrong columns. Same
            // BEGIN NOT ATOMIC form as the probe in Down.
            migrationBuilder.Sql(@"
BEGIN NOT ATOMIC
    IF (SELECT COUNT(*) FROM information_schema.STATISTICS
        WHERE `TABLE_SCHEMA` = DATABASE()
          AND `TABLE_NAME` = 'DeviceTokens'
          AND `INDEX_NAME` = 'IX_DeviceTokens_AppId_InstallationId'
          AND `NON_UNIQUE` = 0
          AND `COLUMN_NAME` IN ('AppId', 'InstallationId')) <> 2 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT =
            'IX_DeviceTokens_AppId_InstallationId is not the expected 2-column UNIQUE index.';
    END IF;
END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Lossy by nature: the old table had no notion of an app or an
            // install, so rolling back merges every app's tokens back into one
            // undifferentiated set.
            //
            // Probe before destroying anything, so the operator gets a sentence
            // rather than an ERROR 1062 several statements later. NULL tokens
            // are excluded: a unique index never collides on them, but GROUP BY
            // would still group them together and report a false positive.
            //
            // The probe is TOCTOU: a still-serving pod can insert a colliding
            // pair between this check and the CREATE UNIQUE INDEX below, and the
            // rollback then fails in exactly the way the probe exists to
            // prevent. Stop writers before rolling back.
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

            // ORDER IS LOAD-BEARING - do not tidy this into the conventional
            // "drop the new columns, then rebuild the old indexes" shape.
            // Recreating IX_DeviceTokens_Token can still fail with ERROR 1062
            // despite the probe above (it is TOCTOU), and MariaDB auto-commits
            // each DDL statement, so a failure here must not find AppId and
            // InstallationId already gone. Rebuilding the old key FIRST, while
            // those columns are still populated, leaves an operator the exact
            // evidence needed to work out which install to keep.
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
