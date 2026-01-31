/*
The MIT License (MIT)

Copyright (c) 2007 - 2025 Microting A/S

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
public class AssignedSiteRuleSetsUTest : DbTestFixture
{
    [Test]
    public async Task AssignedSite_WithWorkingTimeRuleSet_DoesCreate()
    {
        // Arrange
        var workingTimeRuleSet = new WorkingTimeRuleSet
        {
            Name = "Standard 37h Week",
            WeeklyNormalSeconds = 133200,
            MinimumDailyRestSeconds = 39600,
            MinimumWeeklyRestSeconds = 86400,
            WeekStartsOn = 1,
            NightStartSeconds = 72000,
            NightEndSeconds = 21600,
            OvertimeBasis = 1
        };
        await workingTimeRuleSet.Create(DbContext).ConfigureAwait(false);

        var assignedSite = new AssignedSite
        {
            SiteId = 1,
            WorkingTimeRuleSetId = workingTimeRuleSet.Id
        };

        // Act
        await assignedSite.Create(DbContext).ConfigureAwait(false);

        // Assert
        var reloadedSite = await DbContext.AssignedSites
            .Include(a => a.WorkingTimeRuleSet)
            .FirstOrDefaultAsync(a => a.Id == assignedSite.Id)
            .ConfigureAwait(false);

        Assert.That(reloadedSite, Is.Not.Null);
        Assert.That(reloadedSite.WorkingTimeRuleSetId, Is.EqualTo(workingTimeRuleSet.Id));
        Assert.That(reloadedSite.WorkingTimeRuleSet, Is.Not.Null);
        Assert.That(reloadedSite.WorkingTimeRuleSet.Name, Is.EqualTo("Standard 37h Week"));
    }

    [Test]
    public async Task AssignedSite_WithPayRuleSet_DoesCreate()
    {
        // Arrange
        var payRuleSet = new PayRuleSet
        {
            Name = "Sunday Premium Rules"
        };
        await payRuleSet.Create(DbContext).ConfigureAwait(false);

        var assignedSite = new AssignedSite
        {
            SiteId = 1,
            PayRuleSetId = payRuleSet.Id
        };

        // Act
        await assignedSite.Create(DbContext).ConfigureAwait(false);

        // Assert
        var reloadedSite = await DbContext.AssignedSites
            .Include(a => a.PayRuleSet)
            .FirstOrDefaultAsync(a => a.Id == assignedSite.Id)
            .ConfigureAwait(false);

        Assert.That(reloadedSite, Is.Not.Null);
        Assert.That(reloadedSite.PayRuleSetId, Is.EqualTo(payRuleSet.Id));
        Assert.That(reloadedSite.PayRuleSet, Is.Not.Null);
        Assert.That(reloadedSite.PayRuleSet.Name, Is.EqualTo("Sunday Premium Rules"));
    }

    [Test]
    public async Task AssignedSite_WithBothRuleSets_DoesCreate()
    {
        // Arrange
        var workingTimeRuleSet = new WorkingTimeRuleSet
        {
            Name = "Standard 37h Week",
            WeeklyNormalSeconds = 133200,
            MinimumDailyRestSeconds = 39600,
            MinimumWeeklyRestSeconds = 86400,
            WeekStartsOn = 1,
            NightStartSeconds = 72000,
            NightEndSeconds = 21600,
            OvertimeBasis = 1
        };
        await workingTimeRuleSet.Create(DbContext).ConfigureAwait(false);

        var payRuleSet = new PayRuleSet
        {
            Name = "Sunday Premium Rules"
        };
        await payRuleSet.Create(DbContext).ConfigureAwait(false);

        var assignedSite = new AssignedSite
        {
            SiteId = 1,
            WorkingTimeRuleSetId = workingTimeRuleSet.Id,
            PayRuleSetId = payRuleSet.Id
        };

        // Act
        await assignedSite.Create(DbContext).ConfigureAwait(false);

        // Assert
        var reloadedSite = await DbContext.AssignedSites
            .Include(a => a.WorkingTimeRuleSet)
            .Include(a => a.PayRuleSet)
            .FirstOrDefaultAsync(a => a.Id == assignedSite.Id)
            .ConfigureAwait(false);

        Assert.That(reloadedSite, Is.Not.Null);
        Assert.That(reloadedSite.WorkingTimeRuleSetId, Is.EqualTo(workingTimeRuleSet.Id));
        Assert.That(reloadedSite.WorkingTimeRuleSet, Is.Not.Null);
        Assert.That(reloadedSite.WorkingTimeRuleSet.Name, Is.EqualTo("Standard 37h Week"));
        Assert.That(reloadedSite.PayRuleSetId, Is.EqualTo(payRuleSet.Id));
        Assert.That(reloadedSite.PayRuleSet, Is.Not.Null);
        Assert.That(reloadedSite.PayRuleSet.Name, Is.EqualTo("Sunday Premium Rules"));
    }

    [Test]
    public async Task AssignedSite_WithNullRuleSets_DoesCreate()
    {
        // Arrange
        var assignedSite = new AssignedSite
        {
            SiteId = 1,
            WorkingTimeRuleSetId = null,
            PayRuleSetId = null
        };

        // Act
        await assignedSite.Create(DbContext).ConfigureAwait(false);

        // Assert
        var assignedSites = DbContext.AssignedSites.AsNoTracking().ToList();

        Assert.That(assignedSites, Is.Not.Null);
        Assert.That(assignedSites.Count, Is.EqualTo(1));
        Assert.That(assignedSites[0].WorkingTimeRuleSetId, Is.Null);
        Assert.That(assignedSites[0].PayRuleSetId, Is.Null);
    }

    [Test]
    public async Task AssignedSite_UpdateRuleSets_DoesUpdate()
    {
        // Arrange
        var workingTimeRuleSet = new WorkingTimeRuleSet
        {
            Name = "Standard 37h Week",
            WeeklyNormalSeconds = 133200,
            MinimumDailyRestSeconds = 39600,
            MinimumWeeklyRestSeconds = 86400,
            WeekStartsOn = 1,
            NightStartSeconds = 72000,
            NightEndSeconds = 21600,
            OvertimeBasis = 1
        };
        await workingTimeRuleSet.Create(DbContext).ConfigureAwait(false);

        var assignedSite = new AssignedSite
        {
            SiteId = 1,
            WorkingTimeRuleSetId = null,
            PayRuleSetId = null
        };
        await assignedSite.Create(DbContext).ConfigureAwait(false);

        // Act
        assignedSite.WorkingTimeRuleSetId = workingTimeRuleSet.Id;
        await assignedSite.Update(DbContext).ConfigureAwait(false);

        // Assert
        var assignedSites = DbContext.AssignedSites.AsNoTracking().ToList();
        var assignedSiteVersions = DbContext.AssignedSiteVersions.AsNoTracking().ToList();

        Assert.That(assignedSites.Count, Is.EqualTo(1));
        Assert.That(assignedSiteVersions.Count, Is.EqualTo(2));

        Assert.That(assignedSites[0].WorkingTimeRuleSetId, Is.EqualTo(workingTimeRuleSet.Id));
        Assert.That(assignedSites[0].Version, Is.EqualTo(2));
        
        Assert.That(assignedSiteVersions[1].WorkingTimeRuleSetId, Is.EqualTo(workingTimeRuleSet.Id));
    }

    [Test]
    public async Task AssignedSite_WithIsManager_DoesCreate()
    {
        // Arrange
        var assignedSite = new AssignedSite
        {
            SiteId = 1,
            IsManager = true
        };

        // Act
        await assignedSite.Create(DbContext).ConfigureAwait(false);

        // Assert
        var assignedSites = DbContext.AssignedSites.AsNoTracking().ToList();
        var assignedSiteVersions = DbContext.AssignedSiteVersions.AsNoTracking().ToList();

        Assert.That(assignedSites, Is.Not.Null);
        Assert.That(assignedSiteVersions, Is.Not.Null);

        Assert.That(assignedSites.Count, Is.EqualTo(1));
        Assert.That(assignedSiteVersions.Count, Is.EqualTo(1));

        Assert.That(assignedSites[0].IsManager, Is.EqualTo(true));
        Assert.That(assignedSites[0].Version, Is.EqualTo(1));

        Assert.That(assignedSiteVersions[0].IsManager, Is.EqualTo(true));
    }

    [Test]
    public async Task AssignedSite_UpdateIsManager_DoesUpdate()
    {
        // Arrange
        var assignedSite = new AssignedSite
        {
            SiteId = 1,
            IsManager = false
        };
        await assignedSite.Create(DbContext).ConfigureAwait(false);

        // Act
        assignedSite.IsManager = true;
        await assignedSite.Update(DbContext).ConfigureAwait(false);

        // Assert
        var assignedSites = DbContext.AssignedSites.AsNoTracking().ToList();
        var assignedSiteVersions = DbContext.AssignedSiteVersions.AsNoTracking().ToList();

        Assert.That(assignedSites.Count, Is.EqualTo(1));
        Assert.That(assignedSiteVersions.Count, Is.EqualTo(2));

        Assert.That(assignedSites[0].IsManager, Is.EqualTo(true));
        Assert.That(assignedSites[0].Version, Is.EqualTo(2));

        Assert.That(assignedSiteVersions[0].IsManager, Is.EqualTo(false));
        Assert.That(assignedSiteVersions[1].IsManager, Is.EqualTo(true));
    }
}
