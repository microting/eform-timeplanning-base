/*
The MIT License (MIT)

Copyright (c) 2007 - 2026 Microting A/S

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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microting.TimePlanningBase.Infrastructure.Data;
using Microting.TimePlanningBase.Infrastructure.Data.Entities;
using Microting.TimePlanningBase.Infrastructure.Data.Factories;
using NUnit.Framework;

namespace Microting.TimePlanningBase.Tests;

/// <summary>
/// Exercises MakeAssignedSiteResignedAtDateNullable against POPULATED tables.
/// DbTestFixture migrates an empty database, so the migration's cleanup (Up)
/// and NULL backfill (Down) would otherwise match zero rows.
/// Uses its own database so it cannot race DbTestFixture's EnsureDeleted.
/// </summary>
[TestFixture]
public class AssignedSiteResignedAtDateMigrationUTest
{
    private const string DatabaseName = "time-planning-pn-resigned-migration-tests";

    private const string ConnectionString =
        "Server = localhost; port = 3306; Database = " + DatabaseName +
        "; user = root; password = secretpassword; Convert Zero Datetime = true;";

    /// The migration immediately before the one under test.
    private const string PreviousMigration = "20260902121243_AddFlexChainComputedThroughToAssignedSite";

    private const string MigrationUnderTest = "20261010061606_MakeAssignedSiteResignedAtDateNullable";

    private static readonly DateTime Placeholder = new(2026, 1, 15);
    private static readonly DateTime ResignedOn = new(2025, 6, 30);

    private TimePlanningPnDbContext _dbContext;

    [SetUp]
    public void Setup()
    {
        _dbContext = new TimePlanningPnContextFactory().CreateDbContext(new[] { ConnectionString });
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
    public async Task Up_WithExistingRows_DoesClearDateOnlyForLiveNonResignedSites()
    {
        // Arrange - the old NOT NULL schema, where every site carries a date.
        await MigrateToAsync(PreviousMigration).ConfigureAwait(false);
        await new AssignedSite { SiteId = 1, ResignedAtDate = Placeholder }
            .Create(_dbContext).ConfigureAwait(false);
        await new AssignedSite { SiteId = 2, Resigned = true, ResignedAtDate = ResignedOn }
            .Create(_dbContext).ConfigureAwait(false);

        // Act
        await MigrateToAsync(MigrationUnderTest).ConfigureAwait(false);

        // Assert
        var siteDates = await SiteDatesAsync().ConfigureAwait(false);
        var versionDates = await VersionDatesAsync().ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(siteDates, Is.EqualTo(new DateTime?[] { null, ResignedOn }));

            // History is left exactly as it was written.
            Assert.That(versionDates, Is.EqualTo(new DateTime?[] { Placeholder, ResignedOn }));
        });
    }

    [Test]
    public async Task Down_WithNullDates_DoesFillPlaceholderAndKeepRealDates()
    {
        // Arrange
        await MigrateToAsync(MigrationUnderTest).ConfigureAwait(false);
        await new AssignedSite { SiteId = 1 }
            .Create(_dbContext).ConfigureAwait(false);
        await new AssignedSite { SiteId = 2, Resigned = true, ResignedAtDate = ResignedOn }
            .Create(_dbContext).ConfigureAwait(false);

        // Act
        await MigrateToAsync(PreviousMigration).ConfigureAwait(false);

        // Assert
        var sitesNullable = await IsNullableAsync("AssignedSites").ConfigureAwait(false);
        var versionsNullable = await IsNullableAsync("AssignedSiteVersions").ConfigureAwait(false);
        var siteDates = await SiteDatesAsync().ConfigureAwait(false);
        var versionDates = await VersionDatesAsync().ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(sitesNullable, Is.False);
            Assert.That(versionsNullable, Is.False);
            Assert.That(siteDates, Is.EqualTo(new DateTime?[] { DateTime.MinValue, ResignedOn }));
            Assert.That(versionDates, Is.EqualTo(new DateTime?[] { DateTime.MinValue, ResignedOn }));
        });
    }

    /// Always an explicit target, never Migrate() with no argument, so a later
    /// migration cannot silently change what this test exercises.
    private Task MigrateToAsync(string targetMigration) =>
        _dbContext.GetService<IMigrator>().MigrateAsync(targetMigration);

    private Task<DateTime?[]> SiteDatesAsync() =>
        _dbContext.AssignedSites.AsNoTracking()
            .OrderBy(x => x.SiteId).Select(x => x.ResignedAtDate).ToArrayAsync();

    private Task<DateTime?[]> VersionDatesAsync() =>
        _dbContext.AssignedSiteVersions.AsNoTracking()
            .OrderBy(x => x.SiteId).Select(x => x.ResignedAtDate).ToArrayAsync();

    private async Task<bool> IsNullableAsync(string table)
    {
        var connection = _dbContext.Database.GetDbConnection();
        await _dbContext.Database.OpenConnectionAsync().ConfigureAwait(false);
        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText =
                "SELECT IS_NULLABLE FROM information_schema.COLUMNS " +
                "WHERE table_schema = DATABASE() AND table_name = '" + table + "' " +
                "AND column_name = 'ResignedAtDate';";
            return (string)await command.ExecuteScalarAsync().ConfigureAwait(false) == "YES";
        }
        finally
        {
            await _dbContext.Database.CloseConnectionAsync().ConfigureAwait(false);
        }
    }
}
