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
using Microting.TimePlanningBase.Infrastructure.Data.Entities;
using Microting.TimePlanningBase.Infrastructure.Helpers;
using NUnit.Framework;

namespace Microting.TimePlanningBase.Tests;

[TestFixture]
public class FlexChainRepairTests : DbTestFixture
{
    private const int Worker = 4712;
    private const int OtherWorker = 9999;
    private static readonly DateTime D0 = new(2025, 1, 6);

    private async Task<PlanRegistration> Row(int day, double netto, double plan,
        double sumFlexStart = 0, double sumFlexEnd = 0, bool reconciled = false, int worker = Worker)
    {
        var pr = new PlanRegistration
        {
            SdkSitId = worker, Date = D0.AddDays(day), NettoHours = netto,
            NettoHoursInSeconds = ToSeconds(netto), PlanHours = plan, Flex = netto - plan,
            SumFlexStart = sumFlexStart, SumFlexEnd = sumFlexEnd, Reconciled = reconciled
        };
        await pr.Create(DbContext);
        return pr;
    }

    private async Task Site() => await new AssignedSite { SiteId = Worker }.Create(DbContext);

    private PlanRegistration[] Reload() => DbContext.PlanRegistrations.AsNoTracking()
        .Where(x => x.SdkSitId == Worker).OrderBy(x => x.Date).ThenBy(x => x.Id).ToArray();

    private static RestoreLine Restore(PlanRegistration r, double hours, string reason = "recompute") =>
        new(r.Id, r.SdkSitId, r.Version, r.NettoHours, hours, ToSeconds(hours), reason);

    private static int ToSeconds(double hours) => (int)Math.Round(hours * 3600);

    private static Dictionary<int, ExpectedBalance> Expect(params (PlanRegistration row, double start, double end)[] e) =>
        e.ToDictionary(x => x.row.Id, x => new ExpectedBalance(x.row.SdkSitId, x.row.Id, x.start, x.end));

    private int VersionRows() => DbContext.PlanRegistrationVersions.AsNoTracking().Count();

    [Test]
    public async Task Apply_RestoresHours_AndCarriesTheChain()
    {
        await Site();
        var a = await Row(0, 8, 7.5, 0, 0.5);
        var b = await Row(1, 5, 7.5, 0.5, -2.0);      // recomputed wrongly: truth is 8
        var c = await Row(2, 7.5, 7.5, -2.0, -2.0);

        var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, new[] { Restore(b, 8) },
            Expect((a, 0, 0.5), (b, 0.5, 1.0), (c, 1.0, 1.0)), apply: true);

        var rows = Reload();
        Assert.Multiple(() =>
        {
            Assert.That(res.Outcome, Is.EqualTo(RepairOutcome.Applied));
            Assert.That(res.RowsRestored, Is.EqualTo(1));
            Assert.That(rows[1].NettoHours, Is.EqualTo(8));
            Assert.That(rows[1].NettoHoursInSeconds, Is.EqualTo(28800));
            Assert.That(rows[2].SumFlexEnd, Is.EqualTo(1.0).Within(1e-9));
            Assert.That(res.EndBalanceBefore, Is.EqualTo(-2.0).Within(1e-9));
            Assert.That(res.EndBalanceAfter, Is.EqualTo(1.0).Within(1e-9));
            Assert.That(res.BeforeImage.Single(x => x.Id == b.Id).NettoHours, Is.EqualTo(5));
            Assert.That(res.BeforeImage.Single(x => x.Id == b.Id).RepairedVersion, Is.EqualTo(rows[1].Version));
        });
    }

    [Test]
    public async Task DryRun_WritesNothing_AndReportsTheSameCounts()
    {
        await Site();
        var a = await Row(0, 8, 7.5, 0, 0.5);
        var b = await Row(1, 5, 7.5, 0.5, -2.0);
        var before = Reload(); var versionsBefore = VersionRows();

        var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, new[] { Restore(b, 8) },
            Expect((a, 0, 0.5), (b, 0.5, 1.0)), apply: false);

        var after = Reload();
        Assert.Multiple(() =>
        {
            Assert.That(res.Outcome, Is.EqualTo(RepairOutcome.DryRunOk));
            Assert.That(res.RowsRestored, Is.EqualTo(1));
            Assert.That(res.RowsWalked, Is.GreaterThan(0));
            Assert.That(after.Select(x => (x.NettoHours, x.SumFlexEnd, x.Version)),
                Is.EqualTo(before.Select(x => (x.NettoHours, x.SumFlexEnd, x.Version))));
            Assert.That(VersionRows(), Is.EqualTo(versionsBefore));
        });
    }

    [Test]
    public async Task DryRunThenApply_OnSameContext_Applies()
    {
        await Site();
        var a = await Row(0, 8, 7.5, 0, 0.5);
        var b = await Row(1, 5, 7.5, 0.5, -2.0);
        var restores = new[] { Restore(b, 8) };
        var expected = Expect((a, 0, 0.5), (b, 0.5, 1.0));

        await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, restores, expected, apply: false);
        var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, restores, expected, apply: true);

        Assert.Multiple(() =>
        {
            Assert.That(res.Outcome, Is.EqualTo(RepairOutcome.Applied));
            Assert.That(Reload()[1].NettoHours, Is.EqualTo(8));
        });
    }

    [Test]
    public async Task Guard_StaleVersion_FailsAndWritesNothing()
    {
        await Site();
        var a = await Row(0, 8, 7.5, 0, 0.5);
        var b = await Row(1, 5, 7.5, 0.5, -2.0);
        var stale = Restore(b, 8) with { ExpectVersion = b.Version - 1 };
        var versionsBefore = VersionRows();

        var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, new[] { stale },
            Expect((a, 0, 0.5), (b, 0.5, 1.0)), apply: true);

        Assert.Multiple(() =>
        {
            Assert.That(res.Outcome, Is.EqualTo(RepairOutcome.GuardFailed));
            Assert.That(res.GuardFailedIds, Is.EqualTo(new[] { b.Id }));
            Assert.That(Reload()[1].NettoHours, Is.EqualTo(5));
            Assert.That(VersionRows(), Is.EqualTo(versionsBefore));
        });
    }

    [Test]
    public async Task Guard_ChangedHours_Fails()
    {
        await Site();
        var b = await Row(1, 5, 7.5, 0.5, -2.0);
        var line = Restore(b, 8) with { ExpectNettoHours = 5.5 };
        var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, new[] { line },
            new Dictionary<int, ExpectedBalance>(), apply: true);
        Assert.That(res.Outcome, Is.EqualTo(RepairOutcome.GuardFailed));
    }

    [Test]
    public async Task Guard_AnotherWorkersRow_Fails()
    {
        await Site();
        await Row(0, 8, 7.5, 0, 0.5);
        var other = await Row(0, 5, 7.5, 0, -2.5, worker: OtherWorker);
        var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, new[] { Restore(other, 8) },
            new Dictionary<int, ExpectedBalance>(), apply: true);
        Assert.Multiple(() =>
        {
            Assert.That(res.Outcome, Is.EqualTo(RepairOutcome.GuardFailed));
            Assert.That(res.GuardFailedIds, Is.EqualTo(new[] { other.Id }));
        });
    }

    [Test]
    public async Task Guard_RemovedRow_Fails()
    {
        await Site();
        await Row(0, 8, 7.5, 0, 0.5);
        var b = await Row(1, 5, 7.5, 0.5, -2.0);
        var line = Restore(b, 8);
        await b.Delete(DbContext);   // soft-deleted after the restore line was captured
        var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, new[] { line },
            new Dictionary<int, ExpectedBalance>(), apply: true);
        Assert.That(res.Outcome, Is.EqualTo(RepairOutcome.GuardFailed));
    }

    [Test]
    public async Task OpenShiftLine_RestoresZero_AndTheChainFollows()
    {
        await Site();
        var a = await Row(0, 8, 7.5, 0, 0.5);
        var open = await Row(1, -9.58, 7.5, 0.5, -16.58);
        var c = await Row(2, 7.5, 7.5, -16.58, -16.58);

        var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, new[] { Restore(open, 0, "open-shift") },
            Expect((a, 0, 0.5), (open, 0.5, -7.0), (c, -7.0, -7.0)), apply: true);

        var rows = Reload();
        Assert.Multiple(() =>
        {
            Assert.That(res.Outcome, Is.EqualTo(RepairOutcome.Applied));
            Assert.That(rows[1].NettoHours, Is.EqualTo(0));
            Assert.That(rows[1].NettoHoursInSeconds, Is.EqualTo(0));
            Assert.That(rows[2].SumFlexEnd, Is.EqualTo(-7.0).Within(1e-9));
        });
    }

    [Test]
    public async Task NoRestoreLines_TheWalkAloneHealsAStoredBreak()
    {
        await Site();
        var a = await Row(0, 8, 7.5, 0, 0.5);
        var b = await Row(1, 8, 7.5, 3.0, 3.5);        // break: start 3.0 != previous end 0.5
        var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, Array.Empty<RestoreLine>(),
            Expect((a, 0, 0.5), (b, 0.5, 1.0)), apply: true);
        Assert.Multiple(() =>
        {
            Assert.That(res.Outcome, Is.EqualTo(RepairOutcome.Applied));
            Assert.That(res.RowsRestored, Is.EqualTo(0));
            Assert.That(Reload()[1].SumFlexEnd, Is.EqualTo(1.0).Within(1e-9));
        });
    }

    [Test]
    public async Task ExpectedMismatch_RollsBack()
    {
        await Site();
        var a = await Row(0, 8, 7.5, 0, 0.5);
        var b = await Row(1, 5, 7.5, 0.5, -2.0);
        var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, new[] { Restore(b, 8) },
            Expect((a, 0, 0.5), (b, 0.5, 1.5)), apply: true);   // oracle says 1.5, product computes 1.0
        Assert.Multiple(() =>
        {
            Assert.That(res.Outcome, Is.EqualTo(RepairOutcome.Mismatch));
            Assert.That(res.Mismatches.Single().Id, Is.EqualTo(b.Id));
            Assert.That(res.Mismatches.Single().Field, Is.EqualTo("SumFlexEnd"));
            Assert.That(Reload()[1].NettoHours, Is.EqualTo(5));
        });
    }

    [Test]
    public async Task RowWithoutExpectedEntry_IsStillCheckedForContinuity()
    {
        await Site();
        var a = await Row(0, 8, 7.5, 0, 0.5);
        await Row(30, 0, 7.5, 99, 91.5);               // pre-created future row, not in the oracle
        var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, Array.Empty<RestoreLine>(),
            Expect((a, 0, 0.5)), apply: true);
        var rows = Reload();
        Assert.Multiple(() =>
        {
            Assert.That(res.Outcome, Is.EqualTo(RepairOutcome.Applied));
            Assert.That(res.Mismatches, Is.Empty);
            Assert.That(rows[1].SumFlexStart, Is.EqualTo(0.5).Within(1e-9));
            Assert.That(rows[1].SumFlexEnd, Is.EqualTo(-7.0).Within(1e-9));
        });
    }

    [Test]
    public async Task ReconciledDay_IsLocked_AndNothingIsWritten()
    {
        await Site();
        await Row(0, 8, 7.5, 0, 0.5, reconciled: true);
        var b = await Row(1, 5, 7.5, 0.5, -2.0);
        var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, new[] { Restore(b, 8) },
            new Dictionary<int, ExpectedBalance>(), apply: true);
        Assert.Multiple(() =>
        {
            Assert.That(res.Outcome, Is.EqualTo(RepairOutcome.Locked));
            Assert.That(Reload()[1].NettoHours, Is.EqualTo(5));
        });
    }

    [Test]
    public async Task Apply_AddsOneVersionRowPerRestoreAndPerWalkedRow()
    {
        await Site();
        var a = await Row(0, 8, 7.5, 0, 0.5);
        var b = await Row(1, 5, 7.5, 0.5, -2.0);
        var c = await Row(2, 7.5, 7.5, -2.0, -2.0);
        var versionsBefore = VersionRows();

        var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, new[] { Restore(b, 8) },
            Expect((a, 0, 0.5), (b, 0.5, 1.0), (c, 1.0, 1.0)), apply: true);

        Assert.That(VersionRows() - versionsBefore, Is.EqualTo(res.RowsRestored + res.RowsWalked));
    }

    [Test]
    public async Task WorkerWithNoRows_IsANoOp()
    {
        await Site();
        var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, Array.Empty<RestoreLine>(),
            new Dictionary<int, ExpectedBalance>(), apply: false);
        Assert.Multiple(() =>
        {
            Assert.That(res.Outcome, Is.EqualTo(RepairOutcome.DryRunOk));
            Assert.That(res.RowsWalked, Is.EqualTo(0));
            Assert.That(res.EndBalanceAfter, Is.Null);
        });
    }

    [Test]
    public async Task WorkerWithNoRows_RestoreLine_IsGuardFailed()
    {
        await Site();
        var line = new RestoreLine(123456, Worker, 1, 5, 8, ToSeconds(8), "recompute");
        var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, new[] { line },
            new Dictionary<int, ExpectedBalance>(), apply: true);
        Assert.Multiple(() =>
        {
            Assert.That(res.Outcome, Is.EqualTo(RepairOutcome.GuardFailed));
            Assert.That(res.GuardFailedIds, Is.EqualTo(new[] { 123456 }));
        });
    }

    [Test]
    public async Task WorkerWithNoRows_ExpectedEntry_IsMismatch()
    {
        await Site();
        var expected = new Dictionary<int, ExpectedBalance> { [123456] = new(Worker, 123456, 0, 0.5) };
        var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, Array.Empty<RestoreLine>(),
            expected, apply: true);
        Assert.Multiple(() =>
        {
            Assert.That(res.Outcome, Is.EqualTo(RepairOutcome.Mismatch));
            Assert.That(res.Mismatches.Single().Id, Is.EqualTo(123456));
            Assert.That(res.Mismatches.Single().Field, Is.EqualTo("Missing"));
        });
    }

    [Test]
    public async Task Guard_DuplicateRestoreLine_Fails()
    {
        await Site();
        var b = await Row(1, 5, 7.5, 0.5, -2.0);
        var versionsBefore = VersionRows();
        var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, new[] { Restore(b, 8), Restore(b, 8) },
            new Dictionary<int, ExpectedBalance>(), apply: true);
        Assert.Multiple(() =>
        {
            Assert.That(res.Outcome, Is.EqualTo(RepairOutcome.GuardFailed));
            Assert.That(res.GuardFailedIds, Is.EqualTo(new[] { b.Id }));
            Assert.That(Reload()[0].NettoHours, Is.EqualTo(5));
            Assert.That(VersionRows(), Is.EqualTo(versionsBefore));
        });
    }

    [Test]
    public async Task ExpectedRowNoLongerLive_IsAMismatch()
    {
        await Site();
        var a = await Row(0, 8, 7.5, 0, 0.5);
        var b = await Row(1, 7.5, 7.5, 0.5, 0.5);
        var c = await Row(2, 5, 7.5, 0.5, -2.0);
        var expected = Expect((a, 0, 0.5), (b, 0.5, 0.5), (c, 0.5, 1.0));
        await b.Delete(DbContext);   // soft-deleted after the oracle was built
        var versionsBefore = VersionRows();

        var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, new[] { Restore(c, 8) },
            expected, apply: true);

        var rows = Reload().Where(x => x.WorkflowState != Constants.WorkflowStates.Removed).ToArray();
        Assert.Multiple(() =>
        {
            Assert.That(res.Outcome, Is.EqualTo(RepairOutcome.Mismatch));
            Assert.That(res.Mismatches.Single().Id, Is.EqualTo(b.Id));
            Assert.That(res.Mismatches.Single().Field, Is.EqualTo("Missing"));
            Assert.That(rows[1].Id, Is.EqualTo(c.Id));
            Assert.That(rows[1].NettoHours, Is.EqualTo(5));
            Assert.That(VersionRows(), Is.EqualTo(versionsBefore));
        });
    }

    [TestCase(double.NaN)]
    [TestCase(double.PositiveInfinity)]
    [TestCase(-1.0)]
    public void NonFiniteTolerance_Throws(double tolerance)
    {
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => FlexChainRepair.RepairWorkerAsync(DbContext, Worker,
            Array.Empty<RestoreLine>(), new Dictionary<int, ExpectedBalance>(), apply: false, tolerance));
    }

    [Test]
    public async Task Revert_AfterApply_RestoresEveryCapturedColumn()
    {
        await Site();
        var a = await Row(0, 8, 7.5, 0, 0.5);
        var b = await Row(1, 5, 7.5, 0.5, -2.0);
        var original = Reload();
        var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, new[] { Restore(b, 8) },
            Expect((a, 0, 0.5), (b, 0.5, 1.0)), apply: true);

        var rev = await FlexChainRepair.RevertAsync(DbContext, res.BeforeImage, apply: true);

        var rows = Reload();
        Assert.Multiple(() =>
        {
            Assert.That(rev.Reverted, Is.EqualTo(2));
            Assert.That(rev.ChangedIds, Is.Empty);
            Assert.That(rows.Select(x => (x.NettoHours, x.NettoHoursInSeconds, x.Flex, x.SumFlexStart, x.SumFlexEnd)),
                Is.EqualTo(original.Select(x => (x.NettoHours, x.NettoHoursInSeconds, x.Flex, x.SumFlexStart, x.SumFlexEnd))));
        });
    }

    [Test]
    public async Task Revert_SkipsARowChangedAfterTheRepair()
    {
        await Site();
        var a = await Row(0, 8, 7.5, 0, 0.5);
        var b = await Row(1, 5, 7.5, 0.5, -2.0);
        var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, new[] { Restore(b, 8) },
            Expect((a, 0, 0.5), (b, 0.5, 1.0)), apply: true);

        var edited = await DbContext.PlanRegistrations.SingleAsync(x => x.Id == b.Id);
        edited.NettoHours = 9;
        await edited.Update(DbContext);          // Version + 1: the customer edited it after the repair

        var rev = await FlexChainRepair.RevertAsync(DbContext, res.BeforeImage, apply: true);

        Assert.Multiple(() =>
        {
            Assert.That(rev.ChangedIds, Is.EqualTo(new[] { b.Id }));
            Assert.That(Reload()[1].NettoHours, Is.EqualTo(9));
        });
    }

    [Test]
    public async Task Revert_DryRun_WritesNothing()
    {
        await Site();
        var a = await Row(0, 8, 7.5, 0, 0.5);
        var b = await Row(1, 5, 7.5, 0.5, -2.0);
        var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, new[] { Restore(b, 8) },
            Expect((a, 0, 0.5), (b, 0.5, 1.0)), apply: true);

        var rev = await FlexChainRepair.RevertAsync(DbContext, res.BeforeImage, apply: false);

        Assert.Multiple(() =>
        {
            Assert.That(rev.Applied, Is.False);
            Assert.That(rev.Reverted, Is.EqualTo(2));
            Assert.That(Reload()[1].NettoHours, Is.EqualTo(8));
        });
    }

    [Test]
    public async Task Revert_RowOfAnotherWorker_IsNotReverted()
    {
        await Site();
        var other = await Row(0, 5, 7.5, 0, -2.5, worker: OtherWorker);
        var versionsBefore = VersionRows();
        // a before-image row claiming Worker, but its Id is OtherWorker's row
        var forged = new BeforeImageRow(other.Id, Worker, other.Version, other.Version,
            8, 28800, 0.5, 1800, 0, 0, 0.5, 1800);

        var rev = await FlexChainRepair.RevertAsync(DbContext, new[] { forged }, apply: true);

        var row = await DbContext.PlanRegistrations.AsNoTracking().SingleAsync(x => x.Id == other.Id);
        Assert.Multiple(() =>
        {
            Assert.That(rev.ChangedIds, Is.EqualTo(new[] { other.Id }));
            Assert.That(rev.Reverted, Is.EqualTo(0));
            Assert.That(row.NettoHours, Is.EqualTo(5));
            Assert.That(row.SumFlexEnd, Is.EqualTo(-2.5));
            Assert.That(row.Version, Is.EqualTo(other.Version));
            Assert.That(VersionRows(), Is.EqualTo(versionsBefore));
        });
    }

    [Test]
    public void Revert_MixedWorkers_Throws()
    {
        var mixed = new[]
        {
            new BeforeImageRow(1, Worker, 1, 1, 8, 28800, 0.5, 1800, 0, 0, 0.5, 1800),
            new BeforeImageRow(2, OtherWorker, 1, 1, 5, 18000, -2.0, -7200, 0.5, 1800, -2.0, -7200)
        };
        Assert.ThrowsAsync<ArgumentException>(() => FlexChainRepair.RevertAsync(DbContext, mixed, apply: true));
    }

    [Test]
    public void Revert_DuplicateIds_Throws()
    {
        var duplicate = new[]
        {
            new BeforeImageRow(1, Worker, 1, 1, 8, 28800, 0.5, 1800, 0, 0, 0.5, 1800),
            new BeforeImageRow(1, Worker, 2, 2, 9, 32400, 1.5, 5400, 0.5, 1800, 2.0, 7200)
        };
        Assert.ThrowsAsync<ArgumentException>(() => FlexChainRepair.RevertAsync(DbContext, duplicate, apply: true));
    }
}
