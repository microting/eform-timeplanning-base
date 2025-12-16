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
using Microting.TimePlanningBase.Infrastructure.WorkingTime;
using NUnit.Framework;

namespace Microting.TimePlanningBase.Tests;

[TestFixture]
public class WorkingTimeRuleSettingsUTest : DbTestFixture
{
    [Test]
    public async Task WorkingTimeRuleSettings_Create_DoesCreate()
    {
        // Arrange
        var settings = new WorkingTimeRuleSettings
        {
            RuleSetName = "Default Rules",
            WeeklyNormalHours = TimeSpan.FromHours(37),
            DailyNormalHours = TimeSpan.FromMinutes(444),
            MinimumDailyRest = TimeSpan.FromHours(11),
            MinimumWeeklyRest = TimeSpan.FromHours(24),
            WeekStartsOn = DayOfWeek.Monday,
            NightStart = new TimeSpan(17, 0, 0),
            NightEnd = new TimeSpan(6, 0, 0),
            OvertimeBasis = OvertimeBasis.Weekly,
            UnpaidBreakPerDay = TimeSpan.Zero,
            PaidBreakPerDay = TimeSpan.Zero,
            RuleSetVersion = 1
        };

        // Act
        await settings.Create(DbContext).ConfigureAwait(false);

        // Assert
        var settingsRecords = DbContext.WorkingTimeRuleSettings.AsNoTracking().ToList();
        var settingsVersions = DbContext.WorkingTimeRuleSettingsVersions.AsNoTracking().ToList();

        Assert.That(settingsRecords, Is.Not.Null);
        Assert.That(settingsVersions, Is.Not.Null);

        Assert.That(settingsRecords.Count, Is.EqualTo(1));
        Assert.That(settingsVersions.Count, Is.EqualTo(1));

        Assert.That(settingsRecords[0].RuleSetName, Is.EqualTo("Default Rules"));
        Assert.That(settingsRecords[0].WeeklyNormalHours, Is.EqualTo(TimeSpan.FromHours(37)));
        Assert.That(settingsRecords[0].DailyNormalHours, Is.EqualTo(TimeSpan.FromHours(7) + TimeSpan.FromMinutes(24)));
        Assert.That(settingsRecords[0].MinimumDailyRest, Is.EqualTo(TimeSpan.FromHours(11)));
        Assert.That(settingsRecords[0].MinimumWeeklyRest, Is.EqualTo(TimeSpan.FromHours(24)));
        Assert.That(settingsRecords[0].WeekStartsOn, Is.EqualTo(DayOfWeek.Monday));
        Assert.That(settingsRecords[0].NightStart, Is.EqualTo(new TimeSpan(17, 0, 0)));
        Assert.That(settingsRecords[0].NightEnd, Is.EqualTo(new TimeSpan(6, 0, 0)));
        Assert.That(settingsRecords[0].OvertimeBasis, Is.EqualTo(OvertimeBasis.Weekly));
        Assert.That(settingsRecords[0].UnpaidBreakPerDay, Is.EqualTo(TimeSpan.Zero));
        Assert.That(settingsRecords[0].PaidBreakPerDay, Is.EqualTo(TimeSpan.Zero));
        Assert.That(settingsRecords[0].RuleSetVersion, Is.EqualTo(1));
        Assert.That(settingsRecords[0].Version, Is.EqualTo(1));
        Assert.That(settingsRecords[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));

        Assert.That(settingsVersions[0].RuleSetName, Is.EqualTo("Default Rules"));
        Assert.That(settingsVersions[0].WorkingTimeRuleSettingsId, Is.EqualTo(settingsRecords[0].Id));
    }

    [Test]
    public async Task WorkingTimeRuleSettings_Update_DoesUpdate()
    {
        // Arrange
        var settings = new WorkingTimeRuleSettings
        {
            RuleSetName = "Initial Rules",
            WeeklyNormalHours = TimeSpan.FromHours(37),
            DailyNormalHours = TimeSpan.FromMinutes(444),
            OvertimeBasis = OvertimeBasis.Weekly
        };
        await settings.Create(DbContext).ConfigureAwait(false);

        // Act
        settings.RuleSetName = "Updated Rules";
        settings.WeeklyNormalHours = TimeSpan.FromHours(40);
        settings.OvertimeBasis = OvertimeBasis.Daily;
        await settings.Update(DbContext).ConfigureAwait(false);

        // Assert
        var settingsRecords = DbContext.WorkingTimeRuleSettings.AsNoTracking().ToList();
        var settingsVersions = DbContext.WorkingTimeRuleSettingsVersions.AsNoTracking().ToList();

        Assert.That(settingsRecords, Is.Not.Null);
        Assert.That(settingsVersions, Is.Not.Null);

        Assert.That(settingsRecords.Count, Is.EqualTo(1));
        Assert.That(settingsVersions.Count, Is.EqualTo(2));

        Assert.That(settingsRecords[0].RuleSetName, Is.EqualTo("Updated Rules"));
        Assert.That(settingsRecords[0].WeeklyNormalHours, Is.EqualTo(TimeSpan.FromHours(40)));
        Assert.That(settingsRecords[0].OvertimeBasis, Is.EqualTo(OvertimeBasis.Daily));
        Assert.That(settingsRecords[0].Version, Is.EqualTo(2));
        Assert.That(settingsRecords[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));
    }

    [Test]
    public async Task WorkingTimeRuleSettings_Delete_DoesSetWorkflowStateToRemoved()
    {
        // Arrange
        var settings = new WorkingTimeRuleSettings
        {
            RuleSetName = "Test Rules",
            WeeklyNormalHours = TimeSpan.FromHours(37)
        };
        await settings.Create(DbContext).ConfigureAwait(false);

        // Act
        await settings.Delete(DbContext).ConfigureAwait(false);

        // Assert
        var settingsRecords = DbContext.WorkingTimeRuleSettings.AsNoTracking().ToList();
        var settingsVersions = DbContext.WorkingTimeRuleSettingsVersions.AsNoTracking().ToList();

        Assert.That(settingsRecords, Is.Not.Null);
        Assert.That(settingsVersions, Is.Not.Null);

        Assert.That(settingsRecords.Count, Is.EqualTo(1));
        Assert.That(settingsVersions.Count, Is.EqualTo(2));

        Assert.That(settingsRecords[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Removed));
        Assert.That(settingsRecords[0].RuleSetName, Is.EqualTo("Test Rules"));
        Assert.That(settingsRecords[0].Version, Is.EqualTo(2));
    }

    [Test]
    public async Task WorkingTimeRuleSettings_CreateWithDefaultValues_DoesCreate()
    {
        // Arrange
        var settings = new WorkingTimeRuleSettings();

        // Act
        await settings.Create(DbContext).ConfigureAwait(false);

        // Assert
        var settingsRecords = DbContext.WorkingTimeRuleSettings.AsNoTracking().ToList();

        Assert.That(settingsRecords, Is.Not.Null);
        Assert.That(settingsRecords.Count, Is.EqualTo(1));
        Assert.That(settingsRecords[0].RuleSetName, Is.EqualTo("Default"));
        Assert.That(settingsRecords[0].WeeklyNormalHours, Is.EqualTo(TimeSpan.FromHours(37)));
        Assert.That(settingsRecords[0].DailyNormalHours, Is.EqualTo(TimeSpan.FromHours(7) + TimeSpan.FromMinutes(24)));
        Assert.That(settingsRecords[0].MinimumDailyRest, Is.EqualTo(TimeSpan.FromHours(11)));
        Assert.That(settingsRecords[0].MinimumWeeklyRest, Is.EqualTo(TimeSpan.FromHours(24)));
        Assert.That(settingsRecords[0].WeekStartsOn, Is.EqualTo(DayOfWeek.Monday));
        Assert.That(settingsRecords[0].OvertimeBasis, Is.EqualTo(OvertimeBasis.Weekly));
        Assert.That(settingsRecords[0].RuleSetVersion, Is.EqualTo(1));
    }

    [Test]
    public async Task WorkingTimeRuleSettings_CreateWithDailyThenWeeklyOvertimeBasis_DoesCreate()
    {
        // Arrange
        var settings = new WorkingTimeRuleSettings
        {
            RuleSetName = "DailyThenWeekly Rules",
            OvertimeBasis = OvertimeBasis.DailyThenWeekly,
            WeeklyNormalHours = TimeSpan.FromHours(37),
            DailyNormalHours = TimeSpan.FromHours(7.5)
        };

        // Act
        await settings.Create(DbContext).ConfigureAwait(false);

        // Assert
        var settingsRecords = DbContext.WorkingTimeRuleSettings.AsNoTracking().ToList();

        Assert.That(settingsRecords, Is.Not.Null);
        Assert.That(settingsRecords.Count, Is.EqualTo(1));
        Assert.That(settingsRecords[0].RuleSetName, Is.EqualTo("DailyThenWeekly Rules"));
        Assert.That(settingsRecords[0].OvertimeBasis, Is.EqualTo(OvertimeBasis.DailyThenWeekly));
        Assert.That(settingsRecords[0].DailyNormalHours, Is.EqualTo(TimeSpan.FromHours(7.5)));
    }

    [Test]
    public async Task WorkingTimeRuleSettings_CreateWithCustomBreakTimes_DoesCreate()
    {
        // Arrange
        var settings = new WorkingTimeRuleSettings
        {
            RuleSetName = "Break Rules",
            UnpaidBreakPerDay = TimeSpan.FromMinutes(30),
            PaidBreakPerDay = TimeSpan.FromMinutes(15)
        };

        // Act
        await settings.Create(DbContext).ConfigureAwait(false);

        // Assert
        var settingsRecords = DbContext.WorkingTimeRuleSettings.AsNoTracking().ToList();

        Assert.That(settingsRecords, Is.Not.Null);
        Assert.That(settingsRecords.Count, Is.EqualTo(1));
        Assert.That(settingsRecords[0].UnpaidBreakPerDay, Is.EqualTo(TimeSpan.FromMinutes(30)));
        Assert.That(settingsRecords[0].PaidBreakPerDay, Is.EqualTo(TimeSpan.FromMinutes(15)));
    }

    [Test]
    public async Task WorkingTimeRuleSettings_CreateWithCustomWeekStart_DoesCreate()
    {
        // Arrange
        var settings = new WorkingTimeRuleSettings
        {
            RuleSetName = "Sunday Start Rules",
            WeekStartsOn = DayOfWeek.Sunday
        };

        // Act
        await settings.Create(DbContext).ConfigureAwait(false);

        // Assert
        var settingsRecords = DbContext.WorkingTimeRuleSettings.AsNoTracking().ToList();

        Assert.That(settingsRecords, Is.Not.Null);
        Assert.That(settingsRecords.Count, Is.EqualTo(1));
        Assert.That(settingsRecords[0].WeekStartsOn, Is.EqualTo(DayOfWeek.Sunday));
    }

    [Test]
    public async Task WorkingTimeRuleSettings_CreateWithCustomNightShift_DoesCreate()
    {
        // Arrange
        var settings = new WorkingTimeRuleSettings
        {
            RuleSetName = "Night Shift Rules",
            NightStart = new TimeSpan(22, 0, 0),
            NightEnd = new TimeSpan(7, 0, 0)
        };

        // Act
        await settings.Create(DbContext).ConfigureAwait(false);

        // Assert
        var settingsRecords = DbContext.WorkingTimeRuleSettings.AsNoTracking().ToList();

        Assert.That(settingsRecords, Is.Not.Null);
        Assert.That(settingsRecords.Count, Is.EqualTo(1));
        Assert.That(settingsRecords[0].NightStart, Is.EqualTo(new TimeSpan(22, 0, 0)));
        Assert.That(settingsRecords[0].NightEnd, Is.EqualTo(new TimeSpan(7, 0, 0)));
    }
}
