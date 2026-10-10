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
using NUnit.Framework;

namespace Microting.TimePlanningBase.Tests;

/// <summary>
/// AssignedSite.ResignedAtDate holds a value only for a resigned site; a site
/// that is not resigned stores NULL, and every version row mirrors the live row.
/// </summary>
[TestFixture]
public class AssignedSiteResignedAtDateUTest : DbTestFixture
{
    private static readonly DateTime ResignedOn = new(2026, 3, 31);

    [Test]
    public async Task AssignedSite_Create_NotResigned_DoesStoreNullResignedAtDate()
    {
        // Arrange
        var assignedSite = new AssignedSite { SiteId = 1 };

        // Act
        await assignedSite.Create(DbContext).ConfigureAwait(false);

        // Assert
        var stored = await DbContext.AssignedSites.AsNoTracking().SingleAsync().ConfigureAwait(false);
        var version = await DbContext.AssignedSiteVersions.AsNoTracking().SingleAsync().ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(stored.Resigned, Is.False);
            Assert.That(stored.ResignedAtDate, Is.Null);
            Assert.That(version.AssignedSiteId, Is.EqualTo(stored.Id));
            Assert.That(version.ResignedAtDate, Is.Null);
        });
    }

    [Test]
    public async Task AssignedSite_Create_Resigned_DoesStoreResignedAtDate()
    {
        // Arrange
        var assignedSite = new AssignedSite { SiteId = 1, Resigned = true, ResignedAtDate = ResignedOn };

        // Act
        await assignedSite.Create(DbContext).ConfigureAwait(false);

        // Assert
        var stored = await DbContext.AssignedSites.AsNoTracking().SingleAsync().ConfigureAwait(false);
        var version = await DbContext.AssignedSiteVersions.AsNoTracking().SingleAsync().ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(stored.Resigned, Is.True);
            Assert.That(stored.ResignedAtDate, Is.EqualTo(ResignedOn));
            Assert.That(version.ResignedAtDate, Is.EqualTo(ResignedOn));
        });
    }

    [Test]
    public async Task AssignedSite_Update_ResignThenReinstate_DoesMirrorEachStateInVersions()
    {
        // Arrange
        var assignedSite = new AssignedSite { SiteId = 1 };
        await assignedSite.Create(DbContext).ConfigureAwait(false);

        // Act
        assignedSite.Resigned = true;
        assignedSite.ResignedAtDate = ResignedOn;
        await assignedSite.Update(DbContext).ConfigureAwait(false);

        assignedSite.Resigned = false;
        assignedSite.ResignedAtDate = null;
        await assignedSite.Update(DbContext).ConfigureAwait(false);

        // Assert
        var stored = await DbContext.AssignedSites.AsNoTracking().SingleAsync().ConfigureAwait(false);
        var versions = await DbContext.AssignedSiteVersions.AsNoTracking()
            .OrderBy(x => x.Version).ToListAsync().ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(stored.Resigned, Is.False);
            Assert.That(stored.ResignedAtDate, Is.Null);
            Assert.That(versions.Select(x => x.Resigned), Is.EqualTo(new[] { false, true, false }));
            Assert.That(versions.Select(x => x.ResignedAtDate),
                Is.EqualTo(new DateTime?[] { null, ResignedOn, null }));
        });
    }
}
