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
public class HolidayUTest : DbTestFixture
{
    [Test]
    public async Task Holiday_Create_DoesCreate()
    {
        // Arrange
        var holiday = new Holiday
        {
            Name = "Christmas Day",
            Date = new DateTime(2025, 12, 25),
            DayOffPercentage = 100.0
        };

        // Act
        await holiday.Create(DbContext).ConfigureAwait(false);

        // Assert
        var holidays = DbContext.Holidays.AsNoTracking().ToList();
        var holidayVersions = DbContext.HolidayVersions.AsNoTracking().ToList();

        Assert.That(holidays, Is.Not.Null);
        Assert.That(holidayVersions, Is.Not.Null);

        Assert.That(holidays.Count, Is.EqualTo(1));
        Assert.That(holidayVersions.Count, Is.EqualTo(1));

        Assert.That(holidays[0].Name, Is.EqualTo("Christmas Day"));
        Assert.That(holidays[0].Date, Is.EqualTo(new DateTime(2025, 12, 25)));
        Assert.That(holidays[0].DayOffPercentage, Is.EqualTo(100.0));
        Assert.That(holidays[0].Version, Is.EqualTo(1));
        Assert.That(holidays[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));

        Assert.That(holidayVersions[0].Name, Is.EqualTo("Christmas Day"));
        Assert.That(holidayVersions[0].Date, Is.EqualTo(new DateTime(2025, 12, 25)));
        Assert.That(holidayVersions[0].DayOffPercentage, Is.EqualTo(100.0));
        Assert.That(holidayVersions[0].HolidayId, Is.EqualTo(holidays[0].Id));
    }

    [Test]
    public async Task Holiday_Update_DoesUpdate()
    {
        // Arrange
        var holiday = new Holiday
        {
            Name = "New Year's Day",
            Date = new DateTime(2025, 1, 1),
            DayOffPercentage = 100.0
        };
        await holiday.Create(DbContext).ConfigureAwait(false);

        // Act
        holiday.Name = "New Year's Day (Updated)";
        holiday.DayOffPercentage = 50.0;
        await holiday.Update(DbContext).ConfigureAwait(false);

        // Assert
        var holidays = DbContext.Holidays.AsNoTracking().ToList();
        var holidayVersions = DbContext.HolidayVersions.AsNoTracking().ToList();

        Assert.That(holidays, Is.Not.Null);
        Assert.That(holidayVersions, Is.Not.Null);

        Assert.That(holidays.Count, Is.EqualTo(1));
        Assert.That(holidayVersions.Count, Is.EqualTo(2));

        Assert.That(holidays[0].Name, Is.EqualTo("New Year's Day (Updated)"));
        Assert.That(holidays[0].DayOffPercentage, Is.EqualTo(50.0));
        Assert.That(holidays[0].Version, Is.EqualTo(2));
        Assert.That(holidays[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));
    }

    [Test]
    public async Task Holiday_Delete_DoesSetWorkflowStateToRemoved()
    {
        // Arrange
        var holiday = new Holiday
        {
            Name = "Test Holiday",
            Date = new DateTime(2025, 6, 15),
            DayOffPercentage = 75.0
        };
        await holiday.Create(DbContext).ConfigureAwait(false);

        // Act
        await holiday.Delete(DbContext).ConfigureAwait(false);

        // Assert
        var holidays = DbContext.Holidays.AsNoTracking().ToList();
        var holidayVersions = DbContext.HolidayVersions.AsNoTracking().ToList();

        Assert.That(holidays, Is.Not.Null);
        Assert.That(holidayVersions, Is.Not.Null);

        Assert.That(holidays.Count, Is.EqualTo(1));
        Assert.That(holidayVersions.Count, Is.EqualTo(2));

        Assert.That(holidays[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Removed));
        Assert.That(holidays[0].Name, Is.EqualTo("Test Holiday"));
        Assert.That(holidays[0].Version, Is.EqualTo(2));
    }

    [Test]
    public async Task Holiday_CreateWithPartialDay_DoesCreate()
    {
        // Arrange
        var holiday = new Holiday
        {
            Name = "Half Day Holiday",
            Date = new DateTime(2025, 3, 15),
            DayOffPercentage = 50.0
        };

        // Act
        await holiday.Create(DbContext).ConfigureAwait(false);

        // Assert
        var holidays = DbContext.Holidays.AsNoTracking().ToList();

        Assert.That(holidays, Is.Not.Null);
        Assert.That(holidays.Count, Is.EqualTo(1));
        Assert.That(holidays[0].Name, Is.EqualTo("Half Day Holiday"));
        Assert.That(holidays[0].DayOffPercentage, Is.EqualTo(50.0));
    }

    [Test]
    public async Task Holiday_CreateWithDefaultPercentage_DoesCreate()
    {
        // Arrange
        var holiday = new Holiday
        {
            Name = "Default Holiday",
            Date = new DateTime(2025, 7, 4)
        };

        // Act
        await holiday.Create(DbContext).ConfigureAwait(false);

        // Assert
        var holidays = DbContext.Holidays.AsNoTracking().ToList();

        Assert.That(holidays, Is.Not.Null);
        Assert.That(holidays.Count, Is.EqualTo(1));
        Assert.That(holidays[0].Name, Is.EqualTo("Default Holiday"));
        Assert.That(holidays[0].DayOffPercentage, Is.EqualTo(100.0));
    }
}
