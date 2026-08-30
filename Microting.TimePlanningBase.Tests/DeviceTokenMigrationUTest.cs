/*
The MIT License (MIT)

Copyright (c) 2007 - 2025 Microting A/S

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microting.TimePlanningBase.Infrastructure.Data;
using Microting.TimePlanningBase.Infrastructure.Data.Factories;
using NUnit.Framework;

namespace Microting.TimePlanningBase.Tests;

/// <summary>
/// Exercises the DeviceTokenIdentityModel migration against POPULATED tables.
/// <para>
/// DbTestFixture migrates a freshly dropped database, so every other test in
/// this suite runs the migration's DDL against empty tables and its backfill
/// matches zero rows. The data path - the Token to FcmToken rename, the
/// synthetic InstallationId, and the NOT NULL tightening that depends on it -
/// is only covered here.
/// </para>
/// Uses its own database so it cannot race DbTestFixture's EnsureDeleted.
/// </summary>
[TestFixture]
public class DeviceTokenMigrationUTest
{
    private const string DatabaseName = "time-planning-pn-migration-tests";

    private const string ConnectionString =
        "Server = localhost; port = 3306; Database = " + DatabaseName +
        "; user = root; password = secretpassword; Convert Zero Datetime = true;";

    /// The migration immediately before the one under test.
    private const string PreviousMigration = "20260825114900_AddAppBuildNumberToDeviceToken";

    private const string MigrationUnderTest = "20260830081642_DeviceTokenIdentityModel";

    private TimePlanningPnDbContext _dbContext;

    [SetUp]
    public void Setup()
    {
        _dbContext = NewDbContext();
        _dbContext.Database.SetCommandTimeout(300);
        _dbContext.Database.EnsureDeleted();
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext?.Database.EnsureDeleted();
        _dbContext?.Dispose();
    }

    [Test]
    public async Task DeviceTokenIdentityModel_WithExistingRows_DoesBackfillEveryRow()
    {
        // Arrange
        await MigrateToAsync(_dbContext, PreviousMigration).ConfigureAwait(false);
        await SeedOldSchemaRowsAsync(_dbContext).ConfigureAwait(false);

        // Act
        await MigrateToAsync(_dbContext, MigrationUnderTest).ConfigureAwait(false);

        // Assert
        var deviceTokens = _dbContext.DeviceTokens.AsNoTracking().OrderBy(x => x.Id).ToList();

        Assert.That(deviceTokens, Has.Count.EqualTo(4));
        Assert.Multiple(() =>
        {
            Assert.That(deviceTokens.Select(x => x.AppId), Is.All.EqualTo("time"));

            // Derived from the primary key, so it is distinct even for the two
            // rows whose token is NULL. A hash of the token - which the original
            // plan called for - would have produced one shared value for those
            // two and broken the unique index.
            Assert.That(deviceTokens.Select(x => x.InstallationId),
                Is.EqualTo(deviceTokens.Select(x => $"legacy:{x.Id}")));
            Assert.That(deviceTokens.Select(x => x.InstallationId).Distinct().Count(), Is.EqualTo(4));

            // The rename preserved the values rather than dropping the column.
            Assert.That(deviceTokens.Select(x => x.FcmToken),
                Is.EqualTo(new[] { "tok-a", "tok-b", null, null }));

            Assert.That(deviceTokens.Select(x => x.SdkSiteId), Is.EqualTo(new[] { 7, 7, 8, 9 }));
            Assert.That(deviceTokens.Select(x => x.WorkflowState),
                Is.EqualTo(new[] { "created", "created", "created", "removed" }));
        });

        var versions = _dbContext.DeviceTokenVersions.AsNoTracking().OrderBy(x => x.Id).ToList();

        Assert.That(versions, Has.Count.EqualTo(4));
        Assert.Multiple(() =>
        {
            Assert.That(versions.Select(x => x.AppId), Is.All.EqualTo("time"));

            // Keyed off DeviceTokenId, so a version row carries the same
            // synthetic install id as the row it snapshots.
            Assert.That(versions.Select(x => x.InstallationId),
                Is.EqualTo(versions.Select(x => $"legacy:{x.DeviceTokenId}")));
            Assert.That(versions.Select(x => x.FcmToken),
                Is.EqualTo(new[] { "tok-a", "tok-b", null, null }));
        });

        // The identity constraint actually landed as a 2-column UNIQUE index.
        // Asserted explicitly because `CREATE UNIQUE INDEX IF NOT EXISTS`
        // matches on index NAME alone, so a same-named index of the wrong shape
        // would be skipped silently - the case the migration's own probe
        // guards against.
        var uniqueIndexColumns = await UniqueIndexColumnCountAsync(
            _dbContext, "DeviceTokens", "IX_DeviceTokens_AppId_InstallationId").ConfigureAwait(false);

        Assert.That(uniqueIndexColumns, Is.EqualTo(2));

        // The rename also WIDENED the column, 255 -> 512, and until this
        // assertion nothing pinned it: the only other 512 in this file sits
        // inside another test's setup SQL, which is arrangement, not assertion.
        // Without this, changing the migration back to varchar(255) leaves all
        // five tests green while the model snapshot still claims 512.
        var fcmTokenLength = await ScalarAsync(_dbContext,
            "SELECT CHARACTER_MAXIMUM_LENGTH FROM information_schema.COLUMNS " +
            "WHERE table_schema = DATABASE() AND table_name = 'DeviceTokens' " +
            "AND column_name = 'FcmToken';").ConfigureAwait(false);

        Assert.That(fcmTokenLength, Is.EqualTo(512));
    }

    // The boundary no other test reaches: a pod that died one statement PAST
    // the AppId tightening, with InstallationId still nullable. That is the
    // only window in which a row can exist carrying a real AppId and a NULL
    // InstallationId, and it is what the backfill's per-COLUMN `IS NULL` guard
    // exists for - a per-row guard would skip such a row entirely and the
    // InstallationId tightening would then fail.
    [Test]
    public async Task DeviceTokenIdentityModel_ResumesAfterAppIdTightening_DoesSweepNullInstallationId()
    {
        // Arrange
        await MigrateToAsync(_dbContext, PreviousMigration).ConfigureAwait(false);
        await SeedOldSchemaRowsAsync(_dbContext).ConfigureAwait(false);

        // Hand-apply Up as far as the AppId `MODIFY ... NOT NULL`, then stop:
        // the state a pod is left in when it dies there.
        //
        // These are COPIES of the migration's own statements and MUST BE KEPT
        // IN SYNC BY HAND - they are not a reference to them. They differ
        // deliberately only in dropping the IF [NOT] EXISTS guards, which are
        // unnecessary here because each statement is known to apply. Nothing
        // detects drift: change 'time' in the migration's backfill and this
        // test goes on asserting the old SQL, still green.
        foreach (var sql in new[]
                 {
                     "DROP INDEX `IX_DeviceTokens_Token` ON `DeviceTokens`;",
                     "DROP INDEX `IX_DeviceTokens_SdkSiteId` ON `DeviceTokens`;",
                     "ALTER TABLE `DeviceTokens` CHANGE COLUMN " +
                     "`Token` `FcmToken` varchar(512) CHARACTER SET utf8mb4 NULL;",
                     "ALTER TABLE `DeviceTokenVersions` CHANGE COLUMN " +
                     "`Token` `FcmToken` longtext CHARACTER SET utf8mb4 NULL;",
                     "ALTER TABLE `DeviceTokens` ADD COLUMN " +
                     "`AppId` varchar(32) CHARACTER SET utf8mb4 NULL DEFAULT 'time';",
                     "ALTER TABLE `DeviceTokens` ADD COLUMN " +
                     "`InstallationId` varchar(128) CHARACTER SET utf8mb4 NULL;",
                     "ALTER TABLE `DeviceTokenVersions` ADD COLUMN " +
                     "`AppId` longtext CHARACTER SET utf8mb4 NULL;",
                     "ALTER TABLE `DeviceTokenVersions` ADD COLUMN " +
                     "`InstallationId` longtext CHARACTER SET utf8mb4 NULL;",
                     "UPDATE `DeviceTokens` SET " +
                     "`AppId` = COALESCE(`AppId`, 'time'), " +
                     "`InstallationId` = COALESCE(`InstallationId`, CONCAT('legacy:', `Id`)) " +
                     "WHERE `AppId` IS NULL OR `InstallationId` IS NULL;",
                     "UPDATE `DeviceTokenVersions` SET " +
                     "`AppId` = COALESCE(`AppId`, 'time'), " +
                     "`InstallationId` = COALESCE(`InstallationId`, CONCAT('legacy:', `DeviceTokenId`)) " +
                     "WHERE `AppId` IS NULL OR `InstallationId` IS NULL;"
                 })
        {
            await _dbContext.Database.ExecuteSqlRawAsync(sql).ConfigureAwait(false);
        }

        // THE crux of this test, and one statement further than a plain partial
        // application: AppId tightened, InstallationId not. That asymmetric
        // state is the only window in which the row inserted below can exist,
        // and it is where the pod dies. Lifted out of the loop above so it is
        // visible as the boundary rather than unannotated element 11 of an
        // eleven-string array - and so a failure here gets its own stack frame.
        await _dbContext.Database.ExecuteSqlRawAsync(
            "ALTER TABLE `DeviceTokens` " +
            "MODIFY COLUMN `AppId` varchar(32) CHARACTER SET utf8mb4 NOT NULL;")
            .ConfigureAwait(false);

        // An old pod's registration landing in that window. The AppId MUST be
        // named explicitly here, and that necessity IS the finding: `MODIFY
        // COLUMN` restates the whole column definition, so the tightening above
        // has already dropped `DEFAULT 'time'`. From here on an insert that
        // omits AppId fails with ERROR 1364, and re-running Up will NOT bring
        // the default back, because `ADD COLUMN IF NOT EXISTS` matches on name
        // alone. Loud and client-retryable, but the default protects the first
        // pass only.
        await _dbContext.Database.ExecuteSqlRawAsync(
            "INSERT INTO `DeviceTokens` " +
            "(`AppId`, `SdkSiteId`, `FcmToken`, `Platform`, `CreatedAt`, `UpdatedAt`, " +
            " `WorkflowState`, `CreatedByUserId`, `UpdatedByUserId`, `Version`, `AppBuildNumber`) VALUES " +
            "('time', 14, 'window-token', 'android', UTC_TIMESTAMP(), UTC_TIMESTAMP(), " +
            " 'created', 0, 0, 1, 31221);")
            .ConfigureAwait(false);

        // The version snapshot of that same registration, with both identity
        // columns still NULL. DeviceTokenVersions.AppId/InstallationId stay
        // nullable longtext forever - nothing ever tightens them - so unlike its
        // parent row above this one can sit here unstamped indefinitely and only
        // a backfill sweep rescues it.
        //
        // What this pins, and what it does NOT: it pins that Up's version
        // backfill runs on the RESUME path, which no other test covered. It does
        // not uniquely pin the SECOND sweep that follows the tightenings -
        // delete that one, keep the first, and this stays green, because the two
        // sweeps are the same statement and the first reaches any row that
        // already exists when Up starts. The second sweep is live only for a row
        // inserted BETWEEN them, i.e. concurrently with the tightenings, which
        // no deterministic test can arrange.
        await _dbContext.Database.ExecuteSqlRawAsync(
            "INSERT INTO `DeviceTokenVersions` " +
            "(`SdkSiteId`, `FcmToken`, `Platform`, `DeviceTokenId`, `CreatedAt`, `UpdatedAt`, " +
            " `WorkflowState`, `CreatedByUserId`, `UpdatedByUserId`, `Version`, `AppBuildNumber`) " +
            "SELECT `SdkSiteId`, `FcmToken`, `Platform`, `Id`, UTC_TIMESTAMP(), UTC_TIMESTAMP(), " +
            "       'created', 0, 0, 1, `AppBuildNumber` " +
            "FROM `DeviceTokens` WHERE `FcmToken` = 'window-token';")
            .ConfigureAwait(false);

        // Act
        Assert.DoesNotThrowAsync(async () =>
            await MigrateToAsync(_dbContext, MigrationUnderTest).ConfigureAwait(false));

        // Assert
        var deviceTokens = _dbContext.DeviceTokens.AsNoTracking().OrderBy(x => x.Id).ToList();

        Assert.That(deviceTokens, Has.Count.EqualTo(5));

        var uniqueIndexColumns = await UniqueIndexColumnCountAsync(
            _dbContext, "DeviceTokens", "IX_DeviceTokens_AppId_InstallationId").ConfigureAwait(false);

        var stampedWindowVersion = await ScalarAsync(_dbContext,
            "SELECT COUNT(*) FROM `DeviceTokenVersions` WHERE `FcmToken` = 'window-token' " +
            "AND `AppId` = 'time' AND `InstallationId` = CONCAT('legacy:', `DeviceTokenId`);")
            .ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(deviceTokens.Select(x => x.AppId), Is.All.EqualTo("time"));
            Assert.That(deviceTokens.Select(x => x.InstallationId),
                Is.EqualTo(deviceTokens.Select(x => $"legacy:{x.Id}")),
                "the row left with AppId set and InstallationId NULL must be swept");
            Assert.That(uniqueIndexColumns, Is.EqualTo(2));
            Assert.That(stampedWindowVersion, Is.EqualTo(1),
                "the version row left with NULL identity columns must be swept too");
        });
    }

    [Test]
    public async Task DeviceTokenIdentityModel_RunTwice_DoesNotThrow()
    {
        // Arrange
        await MigrateToAsync(_dbContext, PreviousMigration).ConfigureAwait(false);
        await SeedOldSchemaRowsAsync(_dbContext).ConfigureAwait(false);
        await MigrateToAsync(_dbContext, MigrationUnderTest).ConfigureAwait(false);

        // Act
        // MariaDB auto-commits each DDL statement and EF only records the
        // migration after the last one, so a crash partway leaves the schema
        // migrated with no history row and the next Database.Migrate() replays
        // Up from the top. Deleting the history row reproduces exactly that.
        await _dbContext.Database.ExecuteSqlRawAsync(
            $"DELETE FROM `__EFMigrationsHistory` WHERE `MigrationId` = '{MigrationUnderTest}';")
            .ConfigureAwait(false);

        await using var replayContext = NewDbContext();
        Assert.DoesNotThrowAsync(async () =>
            await MigrateToAsync(replayContext, MigrationUnderTest).ConfigureAwait(false));

        // Assert
        var deviceTokens = replayContext.DeviceTokens.AsNoTracking().OrderBy(x => x.Id).ToList();

        Assert.That(deviceTokens, Has.Count.EqualTo(4));
        Assert.Multiple(() =>
        {
            Assert.That(deviceTokens.Select(x => x.AppId), Is.All.EqualTo("time"));
            Assert.That(deviceTokens.Select(x => x.InstallationId),
                Is.EqualTo(deviceTokens.Select(x => $"legacy:{x.Id}")));
            Assert.That(deviceTokens.Select(x => x.FcmToken),
                Is.EqualTo(new[] { "tok-a", "tok-b", null, null }));
        });
    }

    [Test]
    public async Task DeviceTokenIdentityModel_Down_DoesRestoreTheOldSchema()
    {
        // Arrange
        await MigrateToAsync(_dbContext, PreviousMigration).ConfigureAwait(false);
        await SeedOldSchemaRowsAsync(_dbContext).ConfigureAwait(false);
        await MigrateToAsync(_dbContext, MigrationUnderTest).ConfigureAwait(false);

        // Act
        await MigrateToAsync(_dbContext, PreviousMigration).ConfigureAwait(false);

        // Assert
        var tokenColumn = await ColumnCountAsync(_dbContext, "DeviceTokens", "Token").ConfigureAwait(false);
        var fcmTokenColumn = await ColumnCountAsync(_dbContext, "DeviceTokens", "FcmToken").ConfigureAwait(false);
        var appIdColumn = await ColumnCountAsync(_dbContext, "DeviceTokens", "AppId").ConfigureAwait(false);
        var versionTokenColumn = await ColumnCountAsync(_dbContext, "DeviceTokenVersions", "Token").ConfigureAwait(false);

        // The rename carried the values back rather than dropping them.
        var preservedTokens = await ScalarAsync(_dbContext,
            "SELECT COUNT(*) FROM `DeviceTokens` WHERE `Token` IN ('tok-a', 'tok-b');").ConfigureAwait(false);

        // The INDEXES have to come back too, not just the columns. Down rebuilds
        // them with `CREATE [UNIQUE] INDEX IF NOT EXISTS`, which matches on index
        // NAME alone, so a same-named index of the wrong shape would be skipped
        // silently and Down would return without the old unique constraint on
        // Token - the pre-migration schema not actually restored, reported as a
        // clean rollback. Counting UNIQUE STATISTICS rows pins both the
        // uniqueness and the one-column width, matching Down's own probe.
        var tokenUniqueIndexColumns = await UniqueIndexColumnCountAsync(
            _dbContext, "DeviceTokens", "IX_DeviceTokens_Token").ConfigureAwait(false);
        var sdkSiteIdIndexColumns = await ScalarAsync(_dbContext,
            "SELECT COUNT(*) FROM information_schema.STATISTICS " +
            "WHERE table_schema = DATABASE() AND table_name = 'DeviceTokens' " +
            "AND index_name = 'IX_DeviceTokens_SdkSiteId';").ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(tokenColumn, Is.EqualTo(1), "Token should be restored");
            Assert.That(fcmTokenColumn, Is.Zero, "FcmToken should be gone");
            Assert.That(appIdColumn, Is.Zero, "AppId should be gone");
            Assert.That(versionTokenColumn, Is.EqualTo(1), "version Token should be restored");
            Assert.That(preservedTokens, Is.EqualTo(2));
            Assert.That(tokenUniqueIndexColumns, Is.EqualTo(1),
                "the old 1-column UNIQUE index on Token should be rebuilt");
            Assert.That(sdkSiteIdIndexColumns, Is.EqualTo(1),
                "IX_DeviceTokens_SdkSiteId should be rebuilt");
        });
    }

    [Test]
    public async Task DeviceTokenIdentityModel_DownWithDuplicateFcmTokens_DoesThrowBeforeDroppingColumns()
    {
        // Arrange
        await MigrateToAsync(_dbContext, PreviousMigration).ConfigureAwait(false);
        await SeedOldSchemaRowsAsync(_dbContext).ConfigureAwait(false);
        await MigrateToAsync(_dbContext, MigrationUnderTest).ConfigureAwait(false);

        // Two installs sharing one FCM token: legal under the new key, and
        // impossible to represent under the old one.
        await _dbContext.Database.ExecuteSqlRawAsync(
            "UPDATE `DeviceTokens` SET `FcmToken` = 'tok-a' WHERE `SdkSiteId` = 8;")
            .ConfigureAwait(false);

        // Act & Assert
        Assert.ThrowsAsync<MySqlConnector.MySqlException>(async () =>
            await MigrateToAsync(_dbContext, PreviousMigration).ConfigureAwait(false));

        // The probe runs before anything is destroyed, so the columns needed to
        // work out which install to keep are still there.
        var appIdColumn = await ColumnCountAsync(_dbContext, "DeviceTokens", "AppId").ConfigureAwait(false);
        var installationIdColumn =
            await ColumnCountAsync(_dbContext, "DeviceTokens", "InstallationId").ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(appIdColumn, Is.EqualTo(1));
            Assert.That(installationIdColumn, Is.EqualTo(1));
        });
    }

    // Number of columns the index spans - one STATISTICS row per column -
    // counted only while the index is actually UNIQUE, so a non-unique index of
    // the right width cannot pass.
    private static Task<long> UniqueIndexColumnCountAsync(
        TimePlanningPnDbContext dbContext, string tableName, string indexName) =>
        ScalarAsync(dbContext,
            "SELECT COUNT(*) FROM information_schema.STATISTICS " +
            $"WHERE table_schema = DATABASE() AND table_name = '{tableName}' " +
            $"AND index_name = '{indexName}' AND NON_UNIQUE = 0;");

    private static Task<long> ColumnCountAsync(
        TimePlanningPnDbContext dbContext, string tableName, string columnName) =>
        ScalarAsync(dbContext,
            "SELECT COUNT(*) FROM information_schema.COLUMNS " +
            $"WHERE table_schema = DATABASE() AND table_name = '{tableName}' " +
            $"AND column_name = '{columnName}';");

    private static async Task<long> ScalarAsync(TimePlanningPnDbContext dbContext, string sql)
    {
        var connection = dbContext.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync().ConfigureAwait(false);
        }

        await using var command = connection.CreateCommand();
        command.CommandText = sql;

        return Convert.ToInt64(await command.ExecuteScalarAsync().ConfigureAwait(false));
    }

    private static TimePlanningPnDbContext NewDbContext() =>
        new TimePlanningPnContextFactory().CreateDbContext(new[] { ConnectionString });

    // Always an explicit target, never Migrate() with no argument: the day a
    // migration is added after this one, Migrate() would silently start
    // applying that one too, and a failure here would point at the wrong file.
    private static Task MigrateToAsync(TimePlanningPnDbContext dbContext, string targetMigration) =>
        dbContext.GetService<IMigrator>().MigrateAsync(targetMigration);

    /// <summary>
    /// Inserts rows shaped for the pre-migration schema, where the column is
    /// still `Token`. Two rows carry a NULL token, which the old unique index
    /// permitted and which the backfill therefore has to survive.
    /// </summary>
    private static async Task SeedOldSchemaRowsAsync(TimePlanningPnDbContext dbContext)
    {
        await dbContext.Database.ExecuteSqlRawAsync(
            "INSERT INTO `DeviceTokens` " +
            "(`SdkSiteId`, `Token`, `Platform`, `CreatedAt`, `UpdatedAt`, `WorkflowState`, " +
            " `CreatedByUserId`, `UpdatedByUserId`, `Version`, `AppBuildNumber`) VALUES " +
            "(7, 'tok-a', 'android', UTC_TIMESTAMP(), UTC_TIMESTAMP(), 'created', 0, 0, 1, 31221), " +
            "(7, 'tok-b', 'android', UTC_TIMESTAMP(), UTC_TIMESTAMP(), 'created', 0, 0, 1, 31221), " +
            "(8, NULL, 'ios', UTC_TIMESTAMP(), UTC_TIMESTAMP(), 'created', 0, 0, 1, 0), " +
            "(9, NULL, 'ios', UTC_TIMESTAMP(), UTC_TIMESTAMP(), 'removed', 0, 0, 1, 0);")
            .ConfigureAwait(false);

        // One version row per token row, carrying the parent's Id.
        await dbContext.Database.ExecuteSqlRawAsync(
            "INSERT INTO `DeviceTokenVersions` " +
            "(`SdkSiteId`, `Token`, `Platform`, `DeviceTokenId`, `CreatedAt`, `UpdatedAt`, " +
            " `WorkflowState`, `CreatedByUserId`, `UpdatedByUserId`, `Version`, `AppBuildNumber`) " +
            "SELECT `SdkSiteId`, `Token`, `Platform`, `Id`, `CreatedAt`, `UpdatedAt`, " +
            "       `WorkflowState`, 0, 0, 1, `AppBuildNumber` " +
            "FROM `DeviceTokens`;")
            .ConfigureAwait(false);
    }
}
