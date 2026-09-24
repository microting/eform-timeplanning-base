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
using Microting.eForm.Infrastructure.Constants;
using Microting.TimePlanningBase.Infrastructure.Data.Entities;
using Microting.TimePlanningBase.Infrastructure.Helpers;
using NUnit.Framework;

namespace Microting.TimePlanningBase.Tests;

[TestFixture]
public class FlexChainRecomputeTests : DbTestFixture
{
    private const int Worker = 4711;
    private static readonly DateTime D0 = new(2026, 3, 2);

    private async Task<PlanRegistration> Row(int dayOffset, double netto, double plan,
        double sumFlexStart = 0, double sumFlexEnd = 0, bool reconciled = false)
    {
        var pr = new PlanRegistration
        {
            SdkSitId = Worker, Date = D0.AddDays(dayOffset),
            NettoHours = netto, PlanHours = plan, Flex = netto - plan,
            SumFlexStart = sumFlexStart, SumFlexEnd = sumFlexEnd, Reconciled = reconciled
        };
        await pr.Create(DbContext);
        return pr;
    }

    private async Task<AssignedSite> Site(bool oneMinute, DateTime? from = null)
    {
        var site = new AssignedSite { SiteId = Worker, UseOneMinuteIntervals = oneMinute, UseOneMinuteIntervalsFrom = from };
        await site.Create(DbContext);
        return site;
    }

    private PlanRegistration[] Reload() =>
        DbContext.PlanRegistrations.AsNoTracking()
            .Where(x => x.SdkSitId == Worker).OrderBy(x => x.Date).ThenBy(x => x.Id).ToArray();

    [Test]
    public async Task EditMidHistory_CarriesThroughToTheLastPreCreatedRow()
    {
        var site = await Site(oneMinute: false);
        var edited = await Row(0, netto: 9, plan: 7.5, sumFlexEnd: 1.5);   // freshly edited: +1.5
        await Row(1, 7.5, 7.5, 0, 0);                                        // stale chain
        await Row(2, 8.0, 7.5, 0, 0);
        await Row(40, 0, 7.5, 0, 0);                                         // pre-created future row

        var written = await FlexChainRecompute.RunForwardAsync(DbContext, site, Worker, edited.Date);

        var rows = Reload();
        Assert.Multiple(() =>
        {
            Assert.That(written, Is.EqualTo(3));
            Assert.That(rows[1].SumFlexEnd, Is.EqualTo(1.5).Within(1e-9));
            Assert.That(rows[2].SumFlexEnd, Is.EqualTo(2.0).Within(1e-9));
            Assert.That(rows[3].SumFlexStart, Is.EqualTo(2.0).Within(1e-9));
            Assert.That(rows[3].SumFlexEnd, Is.EqualTo(-5.5).Within(1e-9));
        });
    }

    [Test]
    public async Task Walk_NeverRecomputesHours_EvenWithStaleStamps()
    {
        var site = await Site(oneMinute: true, from: D0.AddDays(-30));
        var edited = await Row(0, 8, 8, sumFlexEnd: 0);
        var later = new PlanRegistration
        {
            SdkSitId = Worker, Date = D0.AddDays(1), PlanHours = 7.5,
            Start1Id = 85, Stop1Id = 181,                                     // office: 07:00-15:00
            Start1StartedAt = D0.AddDays(1).AddHours(12),                     // stale device stamps: 1 h
            Stop1StoppedAt = D0.AddDays(1).AddHours(13),
            NettoHours = 8.0, NettoHoursInSeconds = 28800
        };
        await later.Create(DbContext);

        await FlexChainRecompute.RunForwardAsync(DbContext, site, Worker, edited.Date);

        var reloaded = Reload()[1];
        Assert.Multiple(() =>
        {
            Assert.That(reloaded.NettoHoursInSeconds, Is.EqualTo(28800));
            Assert.That(reloaded.NettoHours, Is.EqualTo(8.0));
            Assert.That(reloaded.SumFlexEndInSeconds, Is.EqualTo(1800));
        });
    }

    [Test]
    public async Task ReconciledDays_AreNotWritten_AndTheWalkContinuesFromTheBoundary()
    {
        var site = await Site(oneMinute: false);
        var edited = await Row(0, 9, 7.5, sumFlexEnd: 1.5);
        var locked = await Row(1, 7.5, 7.5, 0, 40.0, reconciled: true);     // stored boundary balance 40
        await Row(2, 8.0, 7.5, 0, 0);

        await FlexChainRecompute.RunForwardAsync(DbContext, site, Worker, edited.Date);

        var rows = Reload();
        Assert.Multiple(() =>
        {
            Assert.That(rows[1].Version, Is.EqualTo(locked.Version), "locked row untouched");
            Assert.That(rows[1].SumFlexEnd, Is.EqualTo(40.0));
            Assert.That(rows[2].SumFlexStart, Is.EqualTo(40.0).Within(1e-9));
            Assert.That(rows[2].SumFlexEnd, Is.EqualTo(40.5).Within(1e-9));
        });
    }

    [Test]
    public async Task ModeBoundaryInsideTheWalk_SeedsTheFirstOneMinuteRowFromTheDecimal()
    {
        var site = await Site(oneMinute: true, from: D0.AddDays(2));
        var edited = await Row(0, 9, 7.5, sumFlexEnd: 1.5);
        await Row(1, 7.5, 7.5);
        var oneMinute = new PlanRegistration
        {
            SdkSitId = Worker, Date = D0.AddDays(2), NettoHours = 8.0, NettoHoursInSeconds = 28800, PlanHours = 7.5
        };
        await oneMinute.Create(DbContext);

        await FlexChainRecompute.RunForwardAsync(DbContext, site, Worker, edited.Date);

        var rows = Reload();
        Assert.Multiple(() =>
        {
            Assert.That(rows[1].SumFlexEndInSeconds, Is.EqualTo(0), "five-minute row carries no seconds");
            Assert.That(rows[2].SumFlexStartInSeconds, Is.EqualTo(5400));
            Assert.That(rows[2].SumFlexEndInSeconds, Is.EqualTo(7200));
        });
    }

    [Test]
    public async Task ConsistentChain_WritesNothing()
    {
        var site = await Site(oneMinute: false);
        var edited = await Row(0, 9, 7.5, sumFlexEnd: 1.5);
        var next = await Row(1, 8, 7.5, sumFlexStart: 1.5, sumFlexEnd: 2.0);
        var versionsBefore = DbContext.PlanRegistrationVersions.Count();

        var written = await FlexChainRecompute.RunForwardAsync(DbContext, site, Worker, edited.Date);

        Assert.Multiple(() =>
        {
            Assert.That(written, Is.EqualTo(0));
            Assert.That(Reload()[1].Version, Is.EqualTo(next.Version));
            Assert.That(DbContext.PlanRegistrationVersions.Count(), Is.EqualTo(versionsBefore));
        });
    }

    [Test]
    public async Task ChangedRows_GetOneVersionBumpAndOneVersionRowEach()
    {
        var site = await Site(oneMinute: false);
        var edited = await Row(0, 9, 7.5, sumFlexEnd: 1.5);
        var a = await Row(1, 8, 7.5);
        var b = await Row(2, 8, 7.5);
        var versionsBefore = DbContext.PlanRegistrationVersions.Count();

        await FlexChainRecompute.RunForwardAsync(DbContext, site, Worker, edited.Date);

        var rows = Reload();
        Assert.Multiple(() =>
        {
            Assert.That(rows[1].Version, Is.EqualTo(a.Version + 1));
            Assert.That(rows[2].Version, Is.EqualTo(b.Version + 1));
            Assert.That(DbContext.PlanRegistrationVersions.Count(), Is.EqualTo(versionsBefore + 2));
            Assert.That(DbContext.PlanRegistrationVersions.AsNoTracking()
                .Where(v => v.PlanRegistrationId == a.Id).OrderByDescending(v => v.Id).First().SumFlexEnd,
                Is.EqualTo(2.0).Within(1e-9));
        });
    }

    [Test]
    public async Task RemovedRows_AreNeitherWalkedNorUsedAsSeed()
    {
        var site = await Site(oneMinute: false);
        var edited = await Row(0, 9, 7.5, sumFlexEnd: 1.5);
        var removed = await Row(1, 20, 0, 0, 99);
        await removed.Delete(DbContext);
        await Row(2, 8, 7.5);

        await FlexChainRecompute.RunForwardAsync(DbContext, site, Worker, edited.Date);

        var live = Reload().Where(x => x.WorkflowState != Constants.WorkflowStates.Removed).ToArray();
        Assert.That(live[1].SumFlexStart, Is.EqualTo(1.5).Within(1e-9));
        Assert.That(DbContext.PlanRegistrations.AsNoTracking().Single(x => x.Id == removed.Id).SumFlexEnd,
            Is.EqualTo(99));
    }

    [Test]
    public async Task SameDateRows_ChainInIdOrder()
    {
        var site = await Site(oneMinute: false);
        var edited = await Row(0, 9, 7.5, sumFlexEnd: 1.5);
        var duplicate = await Row(0, 1, 7.5, 0, 55);
        await Row(1, 7.5, 7.5);

        await FlexChainRecompute.RunForwardAsync(DbContext, site, Worker, edited.Date);

        var rows = Reload();
        Assert.That(rows.Single(x => x.Id == duplicate.Id).SumFlexEnd, Is.EqualTo(-5.0).Within(1e-9));
        Assert.That(rows.Last().SumFlexStart, Is.EqualTo(-5.0).Within(1e-9));
    }

    [Test]
    public async Task FirstRow_KeepsItsStoredOpeningBalance()
    {
        var site = await Site(oneMinute: false);
        await Row(0, 8, 7.5, sumFlexStart: 20, sumFlexEnd: 0);   // opening balance 20, end stale
        await Row(1, 7.5, 7.5);

        await FlexChainRecompute.RunForwardAsync(DbContext, site, Worker, D0);

        var rows = Reload();
        Assert.Multiple(() =>
        {
            Assert.That(rows[0].SumFlexStart, Is.EqualTo(20.0).Within(1e-9));
            Assert.That(rows[0].SumFlexEnd, Is.EqualTo(20.5).Within(1e-9));
            Assert.That(rows[1].SumFlexEnd, Is.EqualTo(20.5).Within(1e-9));
        });
    }

    [Test]
    public async Task NullSite_WalksAsFiveMinute()
    {
        var edited = await Row(0, 9, 7.5, sumFlexEnd: 1.5);
        await Row(1, 8, 7.5);

        var written = await FlexChainRecompute.RunForwardAsync(DbContext, null, Worker, edited.Date);

        Assert.That(written, Is.EqualTo(1));
        Assert.That(Reload()[1].SumFlexEnd, Is.EqualTo(2.0).Within(1e-9));
    }

    [Test]
    public async Task LockBoundaryBeforeTheStart_DoesNotMoveTheStart()
    {
        // the walk trusts rows before the start date: callers pass the earliest date they changed
        var site = await Site(oneMinute: false);
        await Row(0, 7.5, 7.5, 0, 10.0, reconciled: true);   // boundary, balance 10
        var open = await Row(1, 9, 7.5);                      // open, stale
        await Row(2, 8, 7.5);                                 // stale

        await FlexChainRecompute.RunForwardAsync(DbContext, site, Worker, D0.AddDays(2));

        var rows = Reload();
        Assert.Multiple(() =>
        {
            Assert.That(rows[1].Version, Is.EqualTo(open.Version), "walk starts at day 2");
            Assert.That(rows[1].SumFlexEnd, Is.EqualTo(0));
            Assert.That(rows[2].SumFlexStart, Is.EqualTo(0).Within(1e-9));
            Assert.That(rows[2].SumFlexEnd, Is.EqualTo(0.5).Within(1e-9));
        });
    }

    [Test]
    public async Task StartDayIsTheReconciledBoundary_IsNotWritten()
    {
        var site = await Site(oneMinute: false);
        await Row(0, 9, 7.5, 0, 1.5);
        var locked = await Row(1, 8, 7.5, 0, 33.0, reconciled: true);
        await Row(2, 8, 7.5);

        await FlexChainRecompute.RunForwardAsync(DbContext, site, Worker, D0.AddDays(1));

        var rows = Reload();
        Assert.Multiple(() =>
        {
            Assert.That(rows[1].Version, Is.EqualTo(locked.Version), "locked row untouched");
            Assert.That(rows[1].SumFlexEnd, Is.EqualTo(33.0));
            Assert.That(rows[2].SumFlexStart, Is.EqualTo(33.0).Within(1e-9));
            Assert.That(rows[2].SumFlexEnd, Is.EqualTo(33.5).Within(1e-9));
        });
    }

    [Test]
    public async Task FirstRow_OneMinute_KeepsADecimalOnlyOpeningBalance()
    {
        var site = await Site(oneMinute: true, from: D0.AddDays(-30));
        var first = new PlanRegistration
        {
            SdkSitId = Worker, Date = D0, NettoHours = 8, NettoHoursInSeconds = 28800,
            PlanHours = 7.5, Flex = 0.5, SumFlexStart = 20, SumFlexEnd = 0   // opening balance in the decimal only
        };
        await first.Create(DbContext);
        var second = new PlanRegistration
        {
            SdkSitId = Worker, Date = D0.AddDays(1), NettoHours = 7.5, NettoHoursInSeconds = 27000, PlanHours = 7.5
        };
        await second.Create(DbContext);

        await FlexChainRecompute.RunForwardAsync(DbContext, site, Worker, D0);

        var rows = Reload();
        Assert.Multiple(() =>
        {
            Assert.That(rows[0].SumFlexStartInSeconds, Is.EqualTo(72000));
            Assert.That(rows[0].SumFlexEndInSeconds, Is.EqualTo(73800));
            Assert.That(rows[1].SumFlexStartInSeconds, Is.EqualTo(73800));
            Assert.That(rows[1].SumFlexEndInSeconds, Is.EqualTo(73800));
        });
    }
}
