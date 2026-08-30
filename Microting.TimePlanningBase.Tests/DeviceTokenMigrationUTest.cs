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

    private static TimePlanningPnDbContext NewDbContext() =>
        new TimePlanningPnContextFactory().CreateDbContext(new[] { ConnectionString });

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
