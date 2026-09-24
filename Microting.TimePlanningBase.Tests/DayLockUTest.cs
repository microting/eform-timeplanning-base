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
using Microting.eForm.Infrastructure.Constants;
using Microting.TimePlanningBase.Infrastructure.Data.Entities;
using Microting.TimePlanningBase.Infrastructure.Helpers;
using NUnit.Framework;

namespace Microting.TimePlanningBase.Tests;

[TestFixture]
public class DayLockUTest : DbTestFixture
{
    private async Task<PlanRegistration> Row(int sdkSitId, DateTime date, bool reconciled = false)
    {
        var pr = new PlanRegistration { SdkSitId = sdkSitId, Date = date, Reconciled = reconciled };
        await pr.Create(DbContext);
        return pr;
    }

    [Test]
    public async Task LockedThrough_IsTheLatestReconciledDateOfThatWorkerOnly()
    {
        await Row(1, new DateTime(2026, 3, 1), reconciled: true);
        await Row(1, new DateTime(2026, 3, 5), reconciled: true);
        await Row(1, new DateTime(2026, 3, 9));
        await Row(2, new DateTime(2026, 3, 20), reconciled: true);

        Assert.That(await DayLock.LockedThroughAsync(DbContext, 1), Is.EqualTo(new DateTime(2026, 3, 5)));
        Assert.That(await DayLock.LockedThroughAsync(DbContext, 3), Is.Null);
    }

    [Test]
    public async Task LockedThrough_IgnoresRemovedReconciledRows()
    {
        var removed = await Row(1, new DateTime(2026, 3, 9), reconciled: true);
        await removed.Delete(DbContext);

        Assert.That(await DayLock.LockedThroughAsync(DbContext, 1), Is.Null);
    }

    [Test]
    public async Task OpenRows_KeepsOnlyDaysAfterTheBoundary()
    {
        await Row(1, new DateTime(2026, 3, 4));
        await Row(1, new DateTime(2026, 3, 5));
        await Row(1, new DateTime(2026, 3, 6));

        var open = DayLock.OpenRows(DbContext.PlanRegistrations, new DateTime(2026, 3, 5))
            .Select(x => x.Date).ToList();

        Assert.That(open, Is.EqualTo(new[] { new DateTime(2026, 3, 6) }));
    }

    [Test]
    public void IsLocked_ComparesDateOnlyAndTreatsNullAsUnlocked()
    {
        Assert.That(DayLock.IsLocked(new DateTime(2026, 3, 5), new DateTime(2026, 3, 5, 23, 0, 0)), Is.True);
        Assert.That(DayLock.IsLocked(null, new DateTime(2026, 3, 5)), Is.False);
    }
}
