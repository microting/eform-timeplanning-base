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
public class WorkingTimeRuleSetUTest : DbTestFixture
{
    [Test]
    public async Task WorkingTimeRuleSet_Create_DoesCreate()
    {
        // Arrange
        var ruleSet = new WorkingTimeRuleSet
        {
            Name = "Standard Danish 37h Week",
            WeeklyNormalSeconds = 133200, // 37 hours
            DailyNormalSeconds = 27000, // 7.5 hours
            MinimumDailyRestSeconds = 39600, // 11 hours
            MinimumWeeklyRestSeconds = 86400, // 24 hours
            WeekStartsOn = 1, // Monday
            NightStartSeconds = 72000, // 20:00
            NightEndSeconds = 21600, // 06:00
            OvertimeBasis = 1
        };

        // Act
        await ruleSet.Create(DbContext).ConfigureAwait(false);

        // Assert
        var ruleSets = DbContext.WorkingTimeRuleSets.AsNoTracking().ToList();
        var ruleSetVersions = DbContext.WorkingTimeRuleSetVersions.AsNoTracking().ToList();

        Assert.That(ruleSets, Is.Not.Null);
        Assert.That(ruleSetVersions, Is.Not.Null);

        Assert.That(ruleSets.Count, Is.EqualTo(1));
        Assert.That(ruleSetVersions.Count, Is.EqualTo(1));

        Assert.That(ruleSets[0].Name, Is.EqualTo("Standard Danish 37h Week"));
        Assert.That(ruleSets[0].WeeklyNormalSeconds, Is.EqualTo(133200));
        Assert.That(ruleSets[0].DailyNormalSeconds, Is.EqualTo(27000));
        Assert.That(ruleSets[0].MinimumDailyRestSeconds, Is.EqualTo(39600));
        Assert.That(ruleSets[0].MinimumWeeklyRestSeconds, Is.EqualTo(86400));
        Assert.That(ruleSets[0].WeekStartsOn, Is.EqualTo(1));
        Assert.That(ruleSets[0].NightStartSeconds, Is.EqualTo(72000));
        Assert.That(ruleSets[0].NightEndSeconds, Is.EqualTo(21600));
        Assert.That(ruleSets[0].OvertimeBasis, Is.EqualTo(1));
        Assert.That(ruleSets[0].Version, Is.EqualTo(1));
        Assert.That(ruleSets[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));

        Assert.That(ruleSetVersions[0].Name, Is.EqualTo("Standard Danish 37h Week"));
        Assert.That(ruleSetVersions[0].WorkingTimeRuleSetId, Is.EqualTo(ruleSets[0].Id));
    }

    [Test]
    public async Task WorkingTimeRuleSet_Update_DoesUpdate()
    {
        // Arrange
        var ruleSet = new WorkingTimeRuleSet
        {
            Name = "Original Rule Set",
            WeeklyNormalSeconds = 133200,
            MinimumDailyRestSeconds = 39600,
            MinimumWeeklyRestSeconds = 86400,
            WeekStartsOn = 1,
            NightStartSeconds = 72000,
            NightEndSeconds = 21600,
            OvertimeBasis = 1
        };
        await ruleSet.Create(DbContext).ConfigureAwait(false);

        // Act
        ruleSet.Name = "Updated Rule Set";
        ruleSet.WeeklyNormalSeconds = 144000; // 40 hours
        await ruleSet.Update(DbContext).ConfigureAwait(false);

        // Assert
        var ruleSets = DbContext.WorkingTimeRuleSets.AsNoTracking().ToList();
        var ruleSetVersions = DbContext.WorkingTimeRuleSetVersions.AsNoTracking().ToList();

        Assert.That(ruleSets.Count, Is.EqualTo(1));
        Assert.That(ruleSetVersions.Count, Is.EqualTo(2));

        Assert.That(ruleSets[0].Name, Is.EqualTo("Updated Rule Set"));
        Assert.That(ruleSets[0].WeeklyNormalSeconds, Is.EqualTo(144000));
        Assert.That(ruleSets[0].Version, Is.EqualTo(2));
    }

    [Test]
    public async Task WorkingTimeRuleSet_Delete_DoesSetWorkflowStateToRemoved()
    {
        // Arrange
        var ruleSet = new WorkingTimeRuleSet
        {
            Name = "Test Rule Set",
            WeeklyNormalSeconds = 133200,
            MinimumDailyRestSeconds = 39600,
            MinimumWeeklyRestSeconds = 86400,
            WeekStartsOn = 1,
            NightStartSeconds = 72000,
            NightEndSeconds = 21600,
            OvertimeBasis = 1
        };
        await ruleSet.Create(DbContext).ConfigureAwait(false);

        // Act
        await ruleSet.Delete(DbContext).ConfigureAwait(false);

        // Assert
        var ruleSets = DbContext.WorkingTimeRuleSets.AsNoTracking().ToList();
        var ruleSetVersions = DbContext.WorkingTimeRuleSetVersions.AsNoTracking().ToList();

        Assert.That(ruleSets.Count, Is.EqualTo(1));
        Assert.That(ruleSetVersions.Count, Is.EqualTo(2));

        Assert.That(ruleSets[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Removed));
        Assert.That(ruleSets[0].Version, Is.EqualTo(2));
    }

    [Test]
    public async Task WorkingTimeRuleSet_WithNullDailyNormalSeconds_DoesCreate()
    {
        // Arrange
        var ruleSet = new WorkingTimeRuleSet
        {
            Name = "Flexible Daily Hours",
            WeeklyNormalSeconds = 133200,
            DailyNormalSeconds = null, // No daily limit
            MinimumDailyRestSeconds = 39600,
            MinimumWeeklyRestSeconds = 86400,
            WeekStartsOn = 1,
            NightStartSeconds = 72000,
            NightEndSeconds = 21600,
            OvertimeBasis = 1
        };

        // Act
        await ruleSet.Create(DbContext).ConfigureAwait(false);

        // Assert
        var ruleSets = DbContext.WorkingTimeRuleSets.AsNoTracking().ToList();

        Assert.That(ruleSets, Is.Not.Null);
        Assert.That(ruleSets.Count, Is.EqualTo(1));
        Assert.That(ruleSets[0].Name, Is.EqualTo("Flexible Daily Hours"));
        Assert.That(ruleSets[0].DailyNormalSeconds, Is.Null);
    }
}
