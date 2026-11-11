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
using NUnit.Framework;

namespace Microting.TimePlanningBase.Tests;

[TestFixture]
public class PlanPatternUTest : DbTestFixture
{
    private static readonly DateTime Anchor = new(2026, 1, 5);

    private async Task<PlanPattern> CreatePattern()
    {
        var assignedSite = new AssignedSite { SiteId = 1 };
        await assignedSite.Create(DbContext).ConfigureAwait(false);

        var pattern = new PlanPattern
        {
            AssignedSiteId = assignedSite.Id,
            RotationWeeks = 2,
            AnchorDate = Anchor,
            ValidFrom = Anchor
        };
        await pattern.Create(DbContext).ConfigureAwait(false);
        return pattern;
    }

    private async Task<PlanPatternDay> CreateDay(PlanPattern pattern)
    {
        var day = new PlanPatternDay
        {
            PlanPatternId = pattern.Id,
            WeekIndex = 1,
            DayOfWeek = 0,
            PlanText = "7:00-15:00/0.5"
        };
        await day.Create(DbContext).ConfigureAwait(false);
        return day;
    }

    [Test]
    public async Task PlanPattern_Create_WritesVersion()
    {
        var pattern = await CreatePattern().ConfigureAwait(false);

        var patterns = DbContext.PlanPatterns.AsNoTracking().ToList();
        var versions = DbContext.PlanPatternVersions.AsNoTracking().ToList();

        Assert.That(patterns.Count, Is.EqualTo(1));
        Assert.That(versions.Count, Is.EqualTo(1));
        Assert.That(patterns[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));
        Assert.That(patterns[0].Version, Is.EqualTo(1));
        Assert.That(versions[0].PlanPatternId, Is.EqualTo(pattern.Id));
        Assert.That(versions[0].AssignedSiteId, Is.EqualTo(pattern.AssignedSiteId));
        Assert.That(versions[0].RotationWeeks, Is.EqualTo(2));
        Assert.That(versions[0].AnchorDate, Is.EqualTo(Anchor));
        Assert.That(versions[0].ValidFrom, Is.EqualTo(Anchor));
    }

    [Test]
    public async Task PlanPattern_Update_WritesVersion()
    {
        var pattern = await CreatePattern().ConfigureAwait(false);

        pattern.RotationWeeks = 5;
        await pattern.Update(DbContext).ConfigureAwait(false);

        var patterns = DbContext.PlanPatterns.AsNoTracking().ToList();
        var versions = DbContext.PlanPatternVersions.AsNoTracking().OrderBy(v => v.Id).ToList();

        Assert.That(patterns.Count, Is.EqualTo(1));
        Assert.That(patterns[0].RotationWeeks, Is.EqualTo(5));
        Assert.That(patterns[0].Version, Is.EqualTo(2));
        Assert.That(versions.Count, Is.EqualTo(2));
        Assert.That(versions[0].RotationWeeks, Is.EqualTo(2));
        Assert.That(versions[1].RotationWeeks, Is.EqualTo(5));
    }

    [Test]
    public async Task PlanPattern_Delete_WritesVersion()
    {
        var pattern = await CreatePattern().ConfigureAwait(false);

        await pattern.Delete(DbContext).ConfigureAwait(false);

        var patterns = DbContext.PlanPatterns.AsNoTracking().ToList();
        var versions = DbContext.PlanPatternVersions.AsNoTracking().OrderBy(v => v.Id).ToList();

        Assert.That(patterns[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Removed));
        Assert.That(patterns[0].Version, Is.EqualTo(2));
        Assert.That(versions.Count, Is.EqualTo(2));
        Assert.That(versions[1].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Removed));
    }

    [Test]
    public async Task PlanPattern_SeveralRemovedForSameWorker_AreAllowed()
    {
        // The (AssignedSiteId, WorkflowState) index is not unique: replacing a
        // pattern twice leaves two removed rows for the same worker.
        var first = await CreatePattern().ConfigureAwait(false);
        await first.Delete(DbContext).ConfigureAwait(false);

        var second = new PlanPattern
        {
            AssignedSiteId = first.AssignedSiteId,
            RotationWeeks = 1,
            AnchorDate = Anchor,
            ValidFrom = Anchor
        };
        await second.Create(DbContext).ConfigureAwait(false);
        await second.Delete(DbContext).ConfigureAwait(false);

        Assert.That(DbContext.PlanPatterns.AsNoTracking()
            .Count(p => p.WorkflowState == Constants.WorkflowStates.Removed), Is.EqualTo(2));
    }

    [Test]
    public async Task PlanPatternDay_Create_WritesVersion()
    {
        var pattern = await CreatePattern().ConfigureAwait(false);
        var day = await CreateDay(pattern).ConfigureAwait(false);

        var days = DbContext.PlanPatternDays.AsNoTracking().ToList();
        var versions = DbContext.PlanPatternDayVersions.AsNoTracking().ToList();

        Assert.That(days.Count, Is.EqualTo(1));
        Assert.That(versions.Count, Is.EqualTo(1));
        Assert.That(versions[0].PlanPatternDayId, Is.EqualTo(day.Id));
        Assert.That(versions[0].PlanPatternId, Is.EqualTo(pattern.Id));
        Assert.That(versions[0].WeekIndex, Is.EqualTo(1));
        Assert.That(versions[0].DayOfWeek, Is.EqualTo(0));
        Assert.That(versions[0].PlanText, Is.EqualTo("7:00-15:00/0.5"));
    }

    [Test]
    public async Task PlanPatternDay_Update_WritesVersion()
    {
        var pattern = await CreatePattern().ConfigureAwait(false);
        var day = await CreateDay(pattern).ConfigureAwait(false);

        day.PlanText = "8:00-16:00";
        await day.Update(DbContext).ConfigureAwait(false);

        var days = DbContext.PlanPatternDays.AsNoTracking().ToList();
        var versions = DbContext.PlanPatternDayVersions.AsNoTracking().OrderBy(v => v.Id).ToList();

        Assert.That(days[0].PlanText, Is.EqualTo("8:00-16:00"));
        Assert.That(days[0].Version, Is.EqualTo(2));
        Assert.That(versions.Count, Is.EqualTo(2));
        Assert.That(versions[0].PlanText, Is.EqualTo("7:00-15:00/0.5"));
        Assert.That(versions[1].PlanText, Is.EqualTo("8:00-16:00"));
    }

    [Test]
    public async Task PlanPatternDay_Delete_WritesVersion()
    {
        var pattern = await CreatePattern().ConfigureAwait(false);
        var day = await CreateDay(pattern).ConfigureAwait(false);

        await day.Delete(DbContext).ConfigureAwait(false);

        var days = DbContext.PlanPatternDays.AsNoTracking().ToList();
        var versions = DbContext.PlanPatternDayVersions.AsNoTracking().OrderBy(v => v.Id).ToList();

        Assert.That(days[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Removed));
        Assert.That(days[0].Version, Is.EqualTo(2));
        Assert.That(versions.Count, Is.EqualTo(2));
        Assert.That(versions[1].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Removed));
    }

    [Test]
    public async Task PlanPattern_DaysNavigation_LoadsDays()
    {
        var pattern = await CreatePattern().ConfigureAwait(false);
        await CreateDay(pattern).ConfigureAwait(false);

        var loaded = DbContext.PlanPatterns.AsNoTracking()
            .Include(p => p.Days)
            .Single(p => p.Id == pattern.Id);

        Assert.That(loaded.Days.Count, Is.EqualTo(1));
        Assert.That(loaded.Days.Single().PlanText, Is.EqualTo("7:00-15:00/0.5"));
    }
}
