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
using Microting.TimePlanningBase.Infrastructure.Data;
using Microting.TimePlanningBase.Infrastructure.Data.Entities;

namespace Microting.TimePlanningBase.Infrastructure.Helpers;

/// <summary>
/// The reconciled-day ("Afstemt") lock boundary, shared by the plugin and the
/// service. Every day on or before a worker's latest reconciled day is locked.
/// The plugin's DayLockHelper forwards here so there is one definition.
/// </summary>
public static class DayLock
{
    /// <summary>The latest reconciled date for one worker, or null when they have none.</summary>
    public static async Task<DateTime?> LockedThroughAsync(TimePlanningPnDbContext db, int sdkSitId)
        => await BoundaryRows(db)
            .Where(x => x.SdkSitId == sdkSitId)
            .MaxAsync(x => (DateTime?)x.Date);

    public static bool IsLocked(DateTime? lockedThrough, DateTime date)
        => lockedThrough.HasValue && date.Date <= lockedThrough.Value.Date;

    /// <summary>
    /// The rows NOT locked by <paramref name="lockedThrough"/>, as a database
    /// filter — exactly <c>!IsLocked(lockedThrough, x.Date)</c>. A locked row that
    /// is never loaded is never tracked, so no later SaveChanges can flush into it.
    /// </summary>
    public static IQueryable<PlanRegistration> OpenRows(
        IQueryable<PlanRegistration> query, DateTime? lockedThrough)
    {
        if (lockedThrough is not { } boundary)
        {
            return query;
        }

        var firstOpenDay = boundary.Date.AddDays(1);
        return query.Where(x => x.Date >= firstOpenDay);
    }

    private static IQueryable<PlanRegistration> BoundaryRows(TimePlanningPnDbContext db)
        => db.PlanRegistrations
            .Where(x => x.Reconciled)
            .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed);
}
