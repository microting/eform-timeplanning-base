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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microting.eForm.Infrastructure.Constants;
using Microting.TimePlanningBase.Infrastructure.Data.DataMigrations;
using Microting.TimePlanningBase.Infrastructure.Data.Entities;
using NUnit.Framework;

namespace Microting.TimePlanningBase.Tests;

/// <summary>
/// Tests for the § 50 praktikant tier data correction executed by the
/// CorrectPraktikantSection50Tiers migration. The correction rewrites
/// pre-2026-08-07 "Udenlandske praktikanter Landbrug" rule-set snapshots to the
/// corrected catalogue tiers; these tests seed the historically shipped stale
/// shapes and prove the exact target state, idempotency, versioning and scoping.
/// </summary>
[TestFixture]
public class PraktikantSection50TierCorrectionUTest : DbTestFixture
{
    private const string StaldarbejdeBaseName =
        "GLS-A / 3F - Udenlandske praktikanter Landbrug Staldarbejde";

    private const string AndetArbejdeBaseName =
        "GLS-A / 3F - Udenlandske praktikanter Landbrug Andet arbejde";

    private static readonly (int Order, int? UpToSeconds, string PayCode)[] NormalProgression =
    [
        (1, 26640, "NORMAL"),
        (2, 33840, "OVERTIME_50"),
        (3, null, "OVERTIME_80")
    ];

    /// <summary>
    /// The Staldarbejde tier shape as originally shipped (pre-correction): the
    /// weekend/holiday tiers mirror the clock bands and encode no overtime.
    /// </summary>
    private static (string DayCode, (int Order, int? UpToSeconds, string PayCode)[] Tiers)[]
        StaleStaldarbejdeShape() =>
    [
        ("WEEKDAY", NormalProgression),
        ("SATURDAY", [(1, 21600, "SAT_NORMAL"), (2, null, "SAT_ANIMAL_AFTERNOON")]),
        ("SUNDAY", [(1, null, "ANIMAL_SUN_HOLIDAY")]),
        ("HOLIDAY", [(1, null, "ANIMAL_SUN_HOLIDAY")]),
        ("GRUNDLOVSDAG", [(1, null, "ANIMAL_SUN_HOLIDAY")])
    ];

    /// <summary>
    /// The Andet arbejde tier shape as originally shipped: only GRUNDLOVSDAG
    /// deviates from the corrected catalogue (whole day coded as overtime).
    /// </summary>
    private static (string DayCode, (int Order, int? UpToSeconds, string PayCode)[] Tiers)[]
        StaleAndetArbejdeShape() =>
    [
        ("WEEKDAY", NormalProgression),
        ("SATURDAY", NormalProgression),
        ("SUNDAY", [(1, 7200, "OVERTIME_50"), (2, null, "OVERTIME_80")]),
        ("HOLIDAY", [(1, 7200, "OVERTIME_50"), (2, null, "OVERTIME_80")]),
        ("GRUNDLOVSDAG", [(1, 7200, "OVERTIME_50"), (2, null, "OVERTIME_80")])
    ];

    private static (string DayCode, (int Order, int? UpToSeconds, string PayCode)[] Tiers)[]
        CorrectedStaldarbejdeShape() =>
    [
        ("WEEKDAY", NormalProgression),
        ("SATURDAY", [(1, 26640, "SAT_NORMAL"), (2, 33840, "OVERTIME_50"), (3, null, "OVERTIME_80")]),
        ("SUNDAY", [(1, 26640, "ANIMAL_SUN_HOLIDAY"), (2, 33840, "OVERTIME_50"), (3, null, "OVERTIME_80")]),
        ("HOLIDAY", [(1, 26640, "ANIMAL_SUN_HOLIDAY"), (2, 33840, "OVERTIME_50"), (3, null, "OVERTIME_80")]),
        ("GRUNDLOVSDAG", NormalProgression)
    ];

    private static (string DayCode, (int Order, int? UpToSeconds, string PayCode)[] Tiers)[]
        CorrectedAndetArbejdeShape() =>
    [
        ("WEEKDAY", NormalProgression),
        ("SATURDAY", NormalProgression),
        ("SUNDAY", [(1, 7200, "OVERTIME_50"), (2, null, "OVERTIME_80")]),
        ("HOLIDAY", [(1, 7200, "OVERTIME_50"), (2, null, "OVERTIME_80")]),
        ("GRUNDLOVSDAG", NormalProgression)
    ];

    private async Task<PayRuleSet> SeedRuleSet(
        string name,
        (string DayCode, (int Order, int? UpToSeconds, string PayCode)[] Tiers)[] dayRules)
    {
        var ruleSet = new PayRuleSet { Name = name };
        await ruleSet.Create(DbContext).ConfigureAwait(false);

        foreach (var (dayCode, tiers) in dayRules)
        {
            var dayRule = new PayDayRule { PayRuleSetId = ruleSet.Id, DayCode = dayCode };
            await dayRule.Create(DbContext).ConfigureAwait(false);

            foreach (var (order, upToSeconds, payCode) in tiers)
            {
                var tier = new PayTierRule
                {
                    PayDayRuleId = dayRule.Id,
                    Order = order,
                    UpToSeconds = upToSeconds,
                    PayCode = payCode
                };
                await tier.Create(DbContext).ConfigureAwait(false);
            }
        }

        return ruleSet;
    }

    private async Task RunCorrection()
    {
        foreach (var statement in PraktikantSection50TierCorrection.SqlStatements)
        {
            await DbContext.Database.ExecuteSqlRawAsync(statement).ConfigureAwait(false);
        }
    }

    private List<PayTierRule> TiersFor(int ruleSetId, string dayCode)
    {
        var dayRuleId = DbContext.PayDayRules.AsNoTracking()
            .Single(d => d.PayRuleSetId == ruleSetId && d.DayCode == dayCode).Id;

        return DbContext.PayTierRules.AsNoTracking()
            .Where(t => t.PayDayRuleId == dayRuleId
                        && t.WorkflowState != Constants.WorkflowStates.Removed)
            .OrderBy(t => t.Order)
            .ToList();
    }

    private void AssertShape(
        int ruleSetId,
        (string DayCode, (int Order, int? UpToSeconds, string PayCode)[] Tiers)[] expected)
    {
        foreach (var (dayCode, expectedTiers) in expected)
        {
            var tiers = TiersFor(ruleSetId, dayCode);

            Assert.That(tiers.Count, Is.EqualTo(expectedTiers.Length),
                $"tier count for {dayCode}");

            for (var i = 0; i < expectedTiers.Length; i++)
            {
                Assert.That(tiers[i].Order, Is.EqualTo(expectedTiers[i].Order),
                    $"{dayCode} tier {i + 1} Order");
                Assert.That(tiers[i].UpToSeconds, Is.EqualTo(expectedTiers[i].UpToSeconds),
                    $"{dayCode} tier {i + 1} UpToSeconds");
                Assert.That(tiers[i].PayCode, Is.EqualTo(expectedTiers[i].PayCode),
                    $"{dayCode} tier {i + 1} PayCode");
            }
        }
    }

    /// <summary>
    /// Every tier must have a PayTierRuleVersions row for its current Version,
    /// and that snapshot must mirror the tier's current values.
    /// </summary>
    private void AssertVersionRowsConsistent(int ruleSetId)
    {
        var dayRuleIds = DbContext.PayDayRules.AsNoTracking()
            .Where(d => d.PayRuleSetId == ruleSetId)
            .Select(d => d.Id)
            .ToList();

        var tiers = DbContext.PayTierRules.AsNoTracking()
            .Where(t => dayRuleIds.Contains(t.PayDayRuleId))
            .ToList();

        foreach (var tier in tiers)
        {
            var versions = DbContext.PayTierRuleVersions.AsNoTracking()
                .Where(v => v.PayTierRuleId == tier.Id)
                .OrderBy(v => v.Version)
                .ToList();

            Assert.That(versions.Count, Is.EqualTo(tier.Version),
                $"tier {tier.Id} should have one version row per version");

            var latest = versions.Last();
            Assert.That(latest.Version, Is.EqualTo(tier.Version), $"tier {tier.Id} latest version");
            Assert.That(latest.UpToSeconds, Is.EqualTo(tier.UpToSeconds), $"tier {tier.Id} snapshot UpToSeconds");
            Assert.That(latest.PayCode, Is.EqualTo(tier.PayCode), $"tier {tier.Id} snapshot PayCode");
            Assert.That(latest.Order, Is.EqualTo(tier.Order), $"tier {tier.Id} snapshot Order");
            Assert.That(latest.PayDayRuleId, Is.EqualTo(tier.PayDayRuleId), $"tier {tier.Id} snapshot PayDayRuleId");
            Assert.That(latest.WorkflowState, Is.EqualTo(tier.WorkflowState), $"tier {tier.Id} snapshot WorkflowState");
        }
    }

    private (List<PayTierRule> Tiers, List<PayTierRuleVersion> Versions) SnapshotTierTables()
    {
        return (
            DbContext.PayTierRules.AsNoTracking().OrderBy(t => t.Id).ToList(),
            DbContext.PayTierRuleVersions.AsNoTracking().OrderBy(v => v.Id).ToList());
    }

    private static void AssertSnapshotsEqual(
        (List<PayTierRule> Tiers, List<PayTierRuleVersion> Versions) before,
        (List<PayTierRule> Tiers, List<PayTierRuleVersion> Versions) after)
    {
        Assert.That(after.Tiers.Count, Is.EqualTo(before.Tiers.Count), "tier row count");
        Assert.That(after.Versions.Count, Is.EqualTo(before.Versions.Count), "version row count");

        for (var i = 0; i < before.Tiers.Count; i++)
        {
            var b = before.Tiers[i];
            var a = after.Tiers[i];
            Assert.That(
                (a.Id, a.PayDayRuleId, a.Order, a.UpToSeconds, a.PayCode, a.Version, a.WorkflowState, a.UpdatedAt),
                Is.EqualTo(
                    (b.Id, b.PayDayRuleId, b.Order, b.UpToSeconds, b.PayCode, b.Version, b.WorkflowState, b.UpdatedAt)),
                $"tier row {b.Id} changed");
        }

        for (var i = 0; i < before.Versions.Count; i++)
        {
            var b = before.Versions[i];
            var a = after.Versions[i];
            Assert.That(
                (a.Id, a.PayTierRuleId, a.Order, a.UpToSeconds, a.PayCode, a.Version, a.WorkflowState, a.UpdatedAt),
                Is.EqualTo(
                    (b.Id, b.PayTierRuleId, b.Order, b.UpToSeconds, b.PayCode, b.Version, b.WorkflowState, b.UpdatedAt)),
                $"version row {b.Id} changed");
        }
    }

    [Test]
    public async Task StaleStaldarbejde_MigratesToExactTargetState()
    {
        // Arrange - the shape shipped before 2026-08-07, under the old catalogue name
        var ruleSet = await SeedRuleSet(
            $"{StaldarbejdeBaseName} 2024-2026", StaleStaldarbejdeShape());

        // Act
        await RunCorrection();

        // Assert
        AssertShape(ruleSet.Id, CorrectedStaldarbejdeShape());
        AssertVersionRowsConsistent(ruleSet.Id);

        // WEEKDAY was already correct - it must not have been touched at all
        foreach (var tier in TiersFor(ruleSet.Id, "WEEKDAY"))
        {
            Assert.That(tier.Version, Is.EqualTo(1), "untouched WEEKDAY tier version");
        }

        // SATURDAY tier 1 and 2 were rewritten in place, tier 3 was inserted
        var saturday = TiersFor(ruleSet.Id, "SATURDAY");
        Assert.That(saturday[0].Version, Is.EqualTo(2), "rewritten SATURDAY tier 1 version");
        Assert.That(saturday[1].Version, Is.EqualTo(2), "rewritten SATURDAY tier 2 version");
        Assert.That(saturday[2].Version, Is.EqualTo(1), "inserted SATURDAY tier 3 version");
        Assert.That(saturday[2].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));
    }

    [Test]
    public async Task StaleAndetArbejde_MigratesToExactTargetState()
    {
        // Arrange - stale tiers can also live under the renamed 2026-2029 catalogue name
        var ruleSet = await SeedRuleSet(
            $"{AndetArbejdeBaseName} 2026-2029", StaleAndetArbejdeShape());

        // Act
        await RunCorrection();

        // Assert - only GRUNDLOVSDAG was stale; SUNDAY/HOLIDAY stay two-tier all-overtime
        AssertShape(ruleSet.Id, CorrectedAndetArbejdeShape());
        AssertVersionRowsConsistent(ruleSet.Id);

        foreach (var dayCode in new[] { "WEEKDAY", "SATURDAY", "SUNDAY", "HOLIDAY" })
        {
            foreach (var tier in TiersFor(ruleSet.Id, dayCode))
            {
                Assert.That(tier.Version, Is.EqualTo(1), $"already-correct {dayCode} tier version");
            }
        }

        var grundlovsdag = TiersFor(ruleSet.Id, "GRUNDLOVSDAG");
        Assert.That(grundlovsdag[0].Version, Is.EqualTo(2), "rewritten GRUNDLOVSDAG tier 1 version");
        Assert.That(grundlovsdag[1].Version, Is.EqualTo(2), "rewritten GRUNDLOVSDAG tier 2 version");
        Assert.That(grundlovsdag[2].Version, Is.EqualTo(1), "inserted GRUNDLOVSDAG tier 3 version");
    }

    [Test]
    public async Task NameWithEnDashValidityPeriod_IsStillMatched()
    {
        // Arrange - a stored name written with a typographic en dash must normalise
        // to the same preset, mirroring PayRuleSetLock.NormalizePresetName
        var ruleSet = await SeedRuleSet(
            $"{StaldarbejdeBaseName} 2024–2026", StaleStaldarbejdeShape());

        // Act
        await RunCorrection();

        // Assert
        AssertShape(ruleSet.Id, CorrectedStaldarbejdeShape());
    }

    [Test]
    public async Task NameWithEmDashValidityPeriod_IsStillMatched()
    {
        // Arrange - the em dash is the third dash variant the normaliser accepts
        var ruleSet = await SeedRuleSet(
            $"{AndetArbejdeBaseName} 2026—2029", StaleAndetArbejdeShape());

        // Act
        await RunCorrection();

        // Assert
        AssertShape(ruleSet.Id, CorrectedAndetArbejdeShape());
    }

    [Test]
    public async Task ExtraTierBeyondTargetOrders_IsLeftAlone()
    {
        // Arrange - a stale SATURDAY carrying a rogue extra tier at Order 4;
        // the correction rewrites Orders 1-3 and must not delete or alter it
        var ruleSet = await SeedRuleSet(
            $"{StaldarbejdeBaseName} 2024-2026",
            [
                ("SATURDAY",
                [
                    (1, 21600, "SAT_NORMAL"),
                    (2, null, "SAT_ANIMAL_AFTERNOON"),
                    (4, null, "EXTRA")
                ])
            ]);

        // Act
        await RunCorrection();

        // Assert
        var saturday = TiersFor(ruleSet.Id, "SATURDAY");
        Assert.That(saturday.Count, Is.EqualTo(4));
        Assert.That(saturday[0].UpToSeconds, Is.EqualTo(26640));
        Assert.That(saturday[0].PayCode, Is.EqualTo("SAT_NORMAL"));
        Assert.That(saturday[1].UpToSeconds, Is.EqualTo(33840));
        Assert.That(saturday[1].PayCode, Is.EqualTo("OVERTIME_50"));
        Assert.That(saturday[2].UpToSeconds, Is.Null);
        Assert.That(saturday[2].PayCode, Is.EqualTo("OVERTIME_80"));
        Assert.That(saturday[3].Order, Is.EqualTo(4), "extra tier keeps its Order");
        Assert.That(saturday[3].PayCode, Is.EqualTo("EXTRA"), "extra tier keeps its PayCode");
        Assert.That(saturday[3].Version, Is.EqualTo(1), "extra tier is not rewritten");
    }

    [Test]
    public async Task TierMissingItsVersionSnapshot_GetsOneBackfilled()
    {
        // Arrange - an already-correct rule set where one tier anomalously has
        // no version row for its current Version (e.g. a historical
        // version-insert failure); the backfill repairs exactly that gap
        var ruleSet = await SeedRuleSet(
            $"{StaldarbejdeBaseName} 2026-2029", CorrectedStaldarbejdeShape());
        var weekdayTier = TiersFor(ruleSet.Id, "WEEKDAY").First();
        await DbContext.Database.ExecuteSqlRawAsync(
            $"DELETE FROM `PayTierRuleVersions` WHERE `PayTierRuleId` = {weekdayTier.Id}")
            .ConfigureAwait(false);

        // Act
        await RunCorrection();

        // Assert - the tier itself is untouched, but its snapshot is restored
        var after = TiersFor(ruleSet.Id, "WEEKDAY").First();
        Assert.That((after.Version, after.UpdatedAt),
            Is.EqualTo((weekdayTier.Version, weekdayTier.UpdatedAt)), "tier row untouched");
        AssertVersionRowsConsistent(ruleSet.Id);
    }

    [Test]
    public async Task AlreadyCorrectRuleSet_IsUntouched()
    {
        // Arrange - a rule set created from the corrected catalogue
        var ruleSet = await SeedRuleSet(
            $"{StaldarbejdeBaseName} 2026-2029", CorrectedStaldarbejdeShape());
        var before = SnapshotTierTables();

        // Act
        await RunCorrection();

        // Assert - no version churn, no UpdatedAt bump, no new rows
        var after = SnapshotTierTables();
        AssertSnapshotsEqual(before, after);
        AssertShape(ruleSet.Id, CorrectedStaldarbejdeShape());
    }

    [Test]
    public async Task RuleSetWithUnrelatedName_IsUntouched()
    {
        // Arrange - another locked preset, and a name that merely CONTAINS the
        // praktikant name but does not end in it or a validity period
        var dyrehold = await SeedRuleSet(
            "GLS-A / 3F - Jordbrug Dyrehold 2026-2029",
            [
                ("SATURDAY", [(1, 21600, "SAT_NORMAL"), (2, null, "SAT_ANIMAL_AFTERNOON")]),
                ("SUNDAY", [(1, null, "ANIMAL_SUN_HOLIDAY")])
            ]);
        var lookalike = await SeedRuleSet(
            $"{StaldarbejdeBaseName} kopi", StaleStaldarbejdeShape());
        var before = SnapshotTierTables();

        // Act
        await RunCorrection();

        // Assert
        var after = SnapshotTierTables();
        AssertSnapshotsEqual(before, after);
        Assert.That(TiersFor(dyrehold.Id, "SATURDAY").Count, Is.EqualTo(2));
        Assert.That(TiersFor(lookalike.Id, "SUNDAY").Single().UpToSeconds, Is.Null);
    }

    [Test]
    public async Task RemovedRuleSet_IsSkipped()
    {
        // Arrange - a soft-deleted praktikant rule set must not be rewritten
        var ruleSet = await SeedRuleSet(
            $"{StaldarbejdeBaseName} 2024-2026", StaleStaldarbejdeShape());
        var tracked = DbContext.PayRuleSets.Single(s => s.Id == ruleSet.Id);
        await tracked.Delete(DbContext).ConfigureAwait(false);
        var before = SnapshotTierTables();

        // Act
        await RunCorrection();

        // Assert
        var after = SnapshotTierTables();
        AssertSnapshotsEqual(before, after);
    }

    [Test]
    public async Task SecondRun_IsANoOp()
    {
        // Arrange
        var staldarbejde = await SeedRuleSet(
            $"{StaldarbejdeBaseName} 2024-2026", StaleStaldarbejdeShape());
        var andetArbejde = await SeedRuleSet(
            $"{AndetArbejdeBaseName} 2024-2026", StaleAndetArbejdeShape());

        // Act - first run migrates
        await RunCorrection();
        var afterFirstRun = SnapshotTierTables();

        // Act - second run must change nothing
        await RunCorrection();

        // Assert
        var afterSecondRun = SnapshotTierTables();
        AssertSnapshotsEqual(afterFirstRun, afterSecondRun);
        AssertShape(staldarbejde.Id, CorrectedStaldarbejdeShape());
        AssertShape(andetArbejde.Id, CorrectedAndetArbejdeShape());
        AssertVersionRowsConsistent(staldarbejde.Id);
        AssertVersionRowsConsistent(andetArbejde.Id);
    }
}
