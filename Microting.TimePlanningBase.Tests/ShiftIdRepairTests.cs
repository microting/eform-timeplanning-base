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
using Microting.TimePlanningBase.Infrastructure.Data.Entities;
using Microting.TimePlanningBase.Infrastructure.Helpers;
using NUnit.Framework;

namespace Microting.TimePlanningBase.Tests;

[TestFixture]
public class ShiftIdRepairTests : DbTestFixture
{
    private const int Worker = 4712;
    private const int OtherWorker = 9999;
    private static readonly DateTime D0 = new(2025, 1, 6);

    // The corrupted shape: true ids 49 / 61 read back divided by 6.
    private const int BadStart = 8;
    private const int BadStop = 10;
    private const int GoodStart = 49;
    private const int GoodStop = 61;

    private async Task<PlanRegistration> Row(int day, int start1Id = BadStart, int stop1Id = BadStop,
        bool reconciled = false, int worker = Worker, DateTime? startedAt = null, DateTime? stoppedAt = null)
    {
        var pr = new PlanRegistration
        {
            SdkSitId = worker, Date = D0.AddDays(day), Start1Id = start1Id, Stop1Id = stop1Id,
            Start1StartedAt = startedAt, Stop1StoppedAt = stoppedAt,
            NettoHours = 7.5, NettoHoursInSeconds = 27000, PlanHours = 7.4, Flex = 0.1,
            SumFlexStart = 2.25, SumFlexEnd = 2.35, Reconciled = reconciled
        };
        await pr.Create(DbContext);
        return pr;
    }

    private static ShiftIdRepairLine Line(PlanRegistration r, int newStart = GoodStart, int newStop = GoodStop,
        DateTime? newStartedAt = null, DateTime? newStoppedAt = null) =>
        new(r.Id, r.SdkSitId, r.Date, r.Version, r.Start1Id, r.Stop1Id, newStart, newStop,
            newStartedAt, newStoppedAt, "one-minute-div");

    private PlanRegistration Reload(int id) => DbContext.PlanRegistrations.AsNoTracking().Single(x => x.Id == id);

    private int VersionRows(int id) =>
        DbContext.PlanRegistrationVersions.AsNoTracking().Count(x => x.PlanRegistrationId == id);

    [Test]
    public async Task Apply_SetsIds_BumpsVersion_AndWritesAVersionRow()
    {
        var a = await Row(0);
        var versionsBefore = VersionRows(a.Id);

        var res = await ShiftIdRepair.ApplyAsync(DbContext, new[] { Line(a) }, apply: true);

        var after = Reload(a.Id);
        var latest = DbContext.PlanRegistrationVersions.AsNoTracking()
            .Where(x => x.PlanRegistrationId == a.Id).OrderByDescending(x => x.Id).First();
        Assert.Multiple(() =>
        {
            Assert.That(res.Applied, Is.True);
            Assert.That(res.Lines.Single().Outcome, Is.EqualTo(ShiftIdRepairOutcome.Repaired));
            Assert.That((after.Start1Id, after.Stop1Id), Is.EqualTo((GoodStart, GoodStop)));
            Assert.That(after.Version, Is.EqualTo(a.Version + 1));
            Assert.That(VersionRows(a.Id), Is.EqualTo(versionsBefore + 1));
            Assert.That((latest.Start1Id, latest.Stop1Id, latest.Version), Is.EqualTo((GoodStart, GoodStop, after.Version)));
            Assert.That(res.BeforeImage, Is.EqualTo(new[]
            {
                new ShiftIdBeforeImageRow(a.Id, Worker, a.Date, a.Version, after.Version, BadStart, BadStop,
                    null, null, GoodStart, GoodStop)
            }));
        });
    }

    [Test]
    public async Task Apply_WithStamps_SetsThem_AndAnEmptyStampLeavesItAlone()
    {
        var started = D0.AddHours(22);
        var wrongStop = D0.AddHours(23).AddMinutes(55);
        var a = await Row(0, startedAt: started, stoppedAt: wrongStop);
        var b = await Row(1, startedAt: started.AddDays(1), stoppedAt: wrongStop.AddDays(1));
        var rightStop = D0.AddDays(1).AddMinutes(5);

        await ShiftIdRepair.ApplyAsync(DbContext,
            new[] { Line(a, newStoppedAt: rightStop), Line(b) }, apply: true);

        var ra = Reload(a.Id);
        var rb = Reload(b.Id);
        Assert.Multiple(() =>
        {
            Assert.That(ra.Stop1StoppedAt, Is.EqualTo(rightStop));
            Assert.That(ra.Start1StartedAt, Is.EqualTo(started));
            Assert.That(rb.Stop1StoppedAt, Is.EqualTo(wrongStop.AddDays(1)));
            Assert.That(rb.Start1StartedAt, Is.EqualTo(started.AddDays(1)));
            Assert.That((rb.Start1Id, rb.Stop1Id), Is.EqualTo((GoodStart, GoodStop)));
        });
    }

    [Test]
    public async Task DryRun_WritesNothing_ButReportsRepaired()
    {
        var a = await Row(0);
        var versionsBefore = DbContext.PlanRegistrationVersions.AsNoTracking().Count();

        var res = await ShiftIdRepair.ApplyAsync(DbContext, new[] { Line(a, newStoppedAt: D0.AddDays(1)) },
            apply: false);

        var after = Reload(a.Id);
        Assert.Multiple(() =>
        {
            Assert.That(res.Applied, Is.False);
            Assert.That(res.Lines.Single().Outcome, Is.EqualTo(ShiftIdRepairOutcome.Repaired));
            Assert.That(res.BeforeImage, Has.Count.EqualTo(1));
            Assert.That((after.Start1Id, after.Stop1Id, after.Version, after.Stop1StoppedAt),
                Is.EqualTo((BadStart, BadStop, a.Version, (DateTime?)null)));
            Assert.That(DbContext.PlanRegistrationVersions.AsNoTracking().Count(), Is.EqualTo(versionsBefore));
        });
    }

    [Test]
    public async Task DryRunThenApply_OnTheSameContext_Applies()
    {
        var a = await Row(0);
        var lines = new[] { Line(a) };

        await ShiftIdRepair.ApplyAsync(DbContext, lines, apply: false);
        var res = await ShiftIdRepair.ApplyAsync(DbContext, lines, apply: true);

        Assert.Multiple(() =>
        {
            Assert.That(res.Lines.Single().Outcome, Is.EqualTo(ShiftIdRepairOutcome.Repaired));
            Assert.That(Reload(a.Id).Start1Id, Is.EqualTo(GoodStart));
        });
    }

    [TestCase("version")]
    [TestCase("start")]
    [TestCase("stop")]
    public async Task Changed_WhenVersionOrIdsDiffer_AndNothingIsWritten(string field)
    {
        var a = await Row(0);
        var line = field switch
        {
            "version" => Line(a) with { ExpectVersion = a.Version - 1 },
            "start" => Line(a) with { ExpectStart1Id = BadStart + 1 },
            _ => Line(a) with { ExpectStop1Id = BadStop + 1 },
        };
        var versionsBefore = VersionRows(a.Id);

        var res = await ShiftIdRepair.ApplyAsync(DbContext, new[] { line }, apply: true);

        var after = Reload(a.Id);
        Assert.Multiple(() =>
        {
            Assert.That(res.Lines.Single().Outcome, Is.EqualTo(ShiftIdRepairOutcome.Changed));
            Assert.That(res.BeforeImage, Is.Empty);
            Assert.That((after.Start1Id, after.Stop1Id, after.Version), Is.EqualTo((BadStart, BadStop, a.Version)));
            Assert.That(VersionRows(a.Id), Is.EqualTo(versionsBefore));
        });
    }

    [Test]
    public async Task Changed_WhenTheRowWasEditedSinceTheManifest()
    {
        var a = await Row(0);
        var line = Line(a);
        a.Start1Id = 12;
        await a.Update(DbContext);

        var res = await ShiftIdRepair.ApplyAsync(DbContext, new[] { line }, apply: true);

        Assert.Multiple(() =>
        {
            Assert.That(res.Lines.Single().Outcome, Is.EqualTo(ShiftIdRepairOutcome.Changed));
            Assert.That(Reload(a.Id).Start1Id, Is.EqualTo(12));
        });
    }

    [Test]
    public async Task Missing_WhenTheRowIsGone_BelongsToAnotherWorker_OrIsAnotherDate()
    {
        var other = await Row(0, worker: OtherWorker);
        var a = await Row(1);
        var gone = Line(a) with { Id = a.Id + 1000 };
        var wrongWorker = Line(other) with { SdkSitId = Worker };
        var wrongDate = Line(a) with { Date = a.Date.AddDays(1) };

        var res = await ShiftIdRepair.ApplyAsync(DbContext, new[] { gone, wrongWorker }, apply: true);
        var res2 = await ShiftIdRepair.ApplyAsync(DbContext, new[] { wrongDate }, apply: true);

        Assert.Multiple(() =>
        {
            Assert.That(res.Lines.Select(x => x.Outcome),
                Is.EqualTo(new[] { ShiftIdRepairOutcome.Missing, ShiftIdRepairOutcome.Missing }));
            Assert.That(res2.Lines.Single().Outcome, Is.EqualTo(ShiftIdRepairOutcome.Missing));
            Assert.That(Reload(other.Id).Start1Id, Is.EqualTo(BadStart));
            Assert.That(Reload(a.Id).Start1Id, Is.EqualTo(BadStart));
        });
    }

    [Test]
    public async Task Locked_OnAReconciledDay_AndOnADayBeforeIt()
    {
        var early = await Row(0);
        var reconciled = await Row(1, reconciled: true);
        var open = await Row(2);

        var res = await ShiftIdRepair.ApplyAsync(DbContext,
            new[] { Line(early), Line(reconciled), Line(open) }, apply: true);

        Assert.Multiple(() =>
        {
            Assert.That(res.Lines.Select(x => x.Outcome), Is.EqualTo(new[]
            {
                ShiftIdRepairOutcome.Locked, ShiftIdRepairOutcome.Locked, ShiftIdRepairOutcome.Repaired
            }));
            Assert.That(Reload(early.Id).Start1Id, Is.EqualTo(BadStart));
            Assert.That(Reload(reconciled.Id).Start1Id, Is.EqualTo(BadStart));
            Assert.That(Reload(open.Id).Start1Id, Is.EqualTo(GoodStart));
        });
    }

    [Test]
    public async Task ARejectedLine_DoesNotBlockTheOthers()
    {
        var a = await Row(0);
        var b = await Row(1);
        var c = await Row(2);

        var res = await ShiftIdRepair.ApplyAsync(DbContext,
            new[] { Line(a), Line(b) with { ExpectVersion = 99 }, Line(c) with { Id = c.Id + 1000 } },
            apply: true);

        Assert.Multiple(() =>
        {
            Assert.That(res.Lines.Select(x => x.Outcome), Is.EqualTo(new[]
            {
                ShiftIdRepairOutcome.Repaired, ShiftIdRepairOutcome.Changed, ShiftIdRepairOutcome.Missing
            }));
            Assert.That(res.Applied, Is.True);
            Assert.That(Reload(a.Id).Start1Id, Is.EqualTo(GoodStart));
            Assert.That(Reload(b.Id).Start1Id, Is.EqualTo(BadStart));
        });
    }

    [Test]
    public async Task Repair_LeavesNettoHoursAndFlexUntouched()
    {
        var a = await Row(0);

        await ShiftIdRepair.ApplyAsync(DbContext, new[] { Line(a, newStoppedAt: D0.AddDays(1)) }, apply: true);

        var after = Reload(a.Id);
        Assert.That(
            (after.NettoHours, after.NettoHoursInSeconds, after.PlanHours, after.Flex, after.SumFlexStart,
                after.SumFlexEnd, after.Reconciled),
            Is.EqualTo((a.NettoHours, a.NettoHoursInSeconds, a.PlanHours, a.Flex, a.SumFlexStart, a.SumFlexEnd,
                a.Reconciled)));
    }

    [Test]
    public async Task ReversedManifest_RestoresTheBeforeImage()
    {
        var started = D0.AddHours(22);
        var wrongStop = D0.AddHours(23).AddMinutes(55);
        var a = await Row(0, startedAt: started, stoppedAt: wrongStop);
        var b = await Row(1);

        var forward = await ShiftIdRepair.ApplyAsync(DbContext,
            new[] { Line(a, newStoppedAt: D0.AddDays(1).AddMinutes(5)), Line(b) }, apply: true);
        var reversed = forward.BeforeImage.Select(ShiftIdRepair.ReverseLine).ToList();
        var back = await ShiftIdRepair.ApplyAsync(DbContext, reversed, apply: true);

        var ra = Reload(a.Id);
        var rb = Reload(b.Id);
        Assert.Multiple(() =>
        {
            Assert.That(back.Lines.Select(x => x.Outcome), Is.All.EqualTo(ShiftIdRepairOutcome.Repaired));
            Assert.That((ra.Start1Id, ra.Stop1Id, ra.Start1StartedAt, ra.Stop1StoppedAt),
                Is.EqualTo((BadStart, BadStop, (DateTime?)started, (DateTime?)wrongStop)));
            Assert.That((rb.Start1Id, rb.Stop1Id, rb.Stop1StoppedAt),
                Is.EqualTo((BadStart, BadStop, (DateTime?)null)));
            Assert.That(ra.Version, Is.EqualTo(a.Version + 2));
        });
    }

    [Test]
    public void DuplicateIds_AreRejectedBeforeAnythingRuns()
    {
        var line = new ShiftIdRepairLine(1, Worker, D0, 1, BadStart, BadStop, GoodStart, GoodStop, null, null, "x");
        Assert.ThrowsAsync<ArgumentException>(() =>
            ShiftIdRepair.ApplyAsync(DbContext, new[] { line, line }, apply: false));
    }

    [Test]
    public async Task EmptyManifest_IsANoOp()
    {
        var res = await ShiftIdRepair.ApplyAsync(DbContext, Array.Empty<ShiftIdRepairLine>(), apply: true);
        Assert.That(res.Lines, Is.Empty);
    }
}
