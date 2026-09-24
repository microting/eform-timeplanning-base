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
using Microting.TimePlanningBase.Infrastructure.Data;
using Microting.TimePlanningBase.Infrastructure.Data.Entities;

namespace Microting.TimePlanningBase.Infrastructure.Helpers;

/// <summary>
/// Carries a worker's running flex balance forward after a day was changed.
///
/// One walk for every writer (office edit, grid save, flex tab, absence,
/// handover, sheet pull, device submission), so an edit to day D always reaches
/// the worker's LAST row — including rows pre-created for future dates.
///
/// The walk is balance-only (<see cref="FlexChain.CarryChain"/>): worked hours
/// are computed when a day's own inputs change, never by a walk.
/// </summary>
public static class FlexChainRecompute
{
    private const double DecimalTolerance = 1e-9;

    /// <summary>
    /// Walks every non-removed row of the worker dated on or after
    /// <paramref name="fromDateInclusive"/>, ascending (Date, then Id), carrying
    /// the balance in memory from the last live row before it. Reconciled
    /// (locked) days are never loaded for writing: when the boundary is on or
    /// after the start date, the walk starts the day after the boundary and seeds
    /// from the boundary row's stored balance. With no earlier row at all, the
    /// first row keeps its own stored opening balance (SumFlexStart). Only rows
    /// whose chain values change are written: Version + 1, UpdatedAt, and one
    /// version-history row each, in two SaveChanges calls in total.
    ///
    /// Call AFTER saving your own changes, passing the earliest date you changed.
    /// When rows are written the context is flushed, so any other pending change
    /// on it is saved too; when nothing changes, nothing is saved. Save your own
    /// changes first, on THIS context — a row you saved through a different
    /// context would be walked from this context's stale copy.
    /// </summary>
    /// <returns>The number of rows written.</returns>
    public static async Task<int> RunForwardAsync(TimePlanningPnDbContext db,
        AssignedSite? assignedSite, int sdkSitId, DateTime fromDateInclusive)
    {
        var timeline = await OneMinuteModeTimeline.BuildAsync(db, assignedSite);
        var lockedThrough = await DayLock.LockedThroughAsync(db, sdkSitId);

        var live = db.PlanRegistrations
            .Where(x => x.SdkSitId == sdkSitId)
            .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed);

        var from = fromDateInclusive.Date;
        if (lockedThrough is { } boundary && boundary.Date >= from)
        {
            from = boundary.Date.AddDays(1);
        }

        var predecessor = await live.AsNoTracking()
            .Where(x => x.Date < from)
            .OrderByDescending(x => x.Date).ThenByDescending(x => x.Id)
            .FirstOrDefaultAsync();

        var rows = await live
            .Where(x => x.Date >= from)
            .OrderBy(x => x.Date).ThenBy(x => x.Id)
            .ToListAsync();

        if (rows.Count == 0)
        {
            return 0;
        }

        bool? predecessorIsOneMinute;
        if (predecessor != null)
        {
            predecessorIsOneMinute = timeline.WasOneMinuteFor(predecessor);
        }
        else
        {
            // The worker's very first row: its stored SumFlexStart is the opening
            // balance (set via the flex tab), not something to reset to 0.
            var first = rows[0];
            predecessor = new PlanRegistration
            {
                SumFlexEnd = first.SumFlexStart,
                SumFlexEndInSeconds = first.SumFlexStartInSeconds
            };
            // the synthetic seed stands in for the first row itself, so it shares that row's mode
            predecessorIsOneMinute = timeline.WasOneMinuteForRow(first);
        }

        var changed = new List<PlanRegistration>();
        foreach (var row in rows)
        {
            var before = ChainValues.Of(row);
            var rowIsOneMinute = timeline.WasOneMinuteForRow(row);
            FlexChain.CarryChain(row, predecessor, rowIsOneMinute, predecessorIsOneMinute);

            if (before.SameAs(ChainValues.Of(row)))
            {
                before.RestoreInto(row); // leave EF nothing to flush for a float-noise difference
            }
            else
            {
                changed.Add(row);
            }

            predecessor = row;
            predecessorIsOneMinute = rowIsOneMinute;
        }

        if (changed.Count == 0)
        {
            return 0;
        }

        var now = DateTime.UtcNow;
        foreach (var row in changed)
        {
            row.Version += 1;
            row.UpdatedAt = now;
        }
        await db.SaveChangesAsync();

        foreach (var row in changed)
        {
            if (row.CreateVersionSnapshot() is PlanRegistrationVersion version)
            {
                await db.PlanRegistrationVersions.AddAsync(version);
            }
        }
        await db.SaveChangesAsync();

        return changed.Count;
    }

    private readonly record struct ChainValues(
        double Flex, double SumFlexStart, double SumFlexEnd,
        int FlexInSeconds, int SumFlexStartInSeconds, int SumFlexEndInSeconds)
    {
        public static ChainValues Of(PlanRegistration r) => new(
            r.Flex, r.SumFlexStart, r.SumFlexEnd,
            r.FlexInSeconds, r.SumFlexStartInSeconds, r.SumFlexEndInSeconds);

        public bool SameAs(ChainValues o) =>
            Math.Abs(Flex - o.Flex) < DecimalTolerance
            && Math.Abs(SumFlexStart - o.SumFlexStart) < DecimalTolerance
            && Math.Abs(SumFlexEnd - o.SumFlexEnd) < DecimalTolerance
            && FlexInSeconds == o.FlexInSeconds
            && SumFlexStartInSeconds == o.SumFlexStartInSeconds
            && SumFlexEndInSeconds == o.SumFlexEndInSeconds;

        public void RestoreInto(PlanRegistration r)
        {
            r.Flex = Flex;
            r.SumFlexStart = SumFlexStart;
            r.SumFlexEnd = SumFlexEnd;
            r.FlexInSeconds = FlexInSeconds;
            r.SumFlexStartInSeconds = SumFlexStartInSeconds;
            r.SumFlexEndInSeconds = SumFlexEndInSeconds;
        }
    }
}
