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
public class PlanRegistrationPayLineUTest : DbTestFixture
{
    [Test]
    public async Task PlanRegistrationPayLine_Create_DoesCreate()
    {
        // Arrange
        var planRegistration = new PlanRegistration
        {
            SdkSitId = 1,
            Date = DateTime.UtcNow.Date,
            PlanText = "Test Plan",
            PlanHours = 8.0,
            PlanHoursInSeconds = 28800
        };
        await planRegistration.Create(DbContext).ConfigureAwait(false);

        var payLine = new PlanRegistrationPayLine
        {
            PlanRegistrationId = planRegistration.Id,
            PayCode = "NORMAL",
            Hours = 8.0,
            HoursInSeconds = 28800,
            PayRuleSetId = 1,
            CalculatedAt = DateTime.UtcNow
        };

        // Act
        await payLine.Create(DbContext).ConfigureAwait(false);

        // Assert
        var payLines = DbContext.PlanRegistrationPayLines.AsNoTracking().ToList();
        var payLineVersions = DbContext.PlanRegistrationPayLineVersions.AsNoTracking().ToList();

        Assert.That(payLines, Is.Not.Null);
        Assert.That(payLineVersions, Is.Not.Null);

        Assert.That(payLines.Count, Is.EqualTo(1));
        Assert.That(payLineVersions.Count, Is.EqualTo(1));

        Assert.That(payLines[0].PlanRegistrationId, Is.EqualTo(planRegistration.Id));
        Assert.That(payLines[0].PayCode, Is.EqualTo("NORMAL"));
        Assert.That(payLines[0].Hours, Is.EqualTo(8.0));
        Assert.That(payLines[0].HoursInSeconds, Is.EqualTo(28800));
        Assert.That(payLines[0].PayRuleSetId, Is.EqualTo(1));
        Assert.That(payLines[0].Version, Is.EqualTo(1));
        Assert.That(payLines[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));

        Assert.That(payLineVersions[0].PlanRegistrationPayLineId, Is.EqualTo(payLines[0].Id));
    }

    [Test]
    public async Task PlanRegistrationPayLine_WithNavigation_LoadsRelatedPlanRegistration()
    {
        // Arrange
        var planRegistration = new PlanRegistration
        {
            SdkSitId = 1,
            Date = DateTime.UtcNow.Date,
            PlanText = "Test Plan",
            PlanHours = 8.0,
            PlanHoursInSeconds = 28800
        };
        await planRegistration.Create(DbContext).ConfigureAwait(false);

        var payLine = new PlanRegistrationPayLine
        {
            PlanRegistrationId = planRegistration.Id,
            PayCode = "NORMAL",
            Hours = 8.0,
            HoursInSeconds = 28800,
            CalculatedAt = DateTime.UtcNow
        };
        await payLine.Create(DbContext).ConfigureAwait(false);

        // Act - Reload with includes
        var reloadedPayLine = await DbContext.PlanRegistrationPayLines
            .Include(p => p.PlanRegistration)
            .FirstOrDefaultAsync(p => p.Id == payLine.Id)
            .ConfigureAwait(false);

        // Assert
        Assert.That(reloadedPayLine, Is.Not.Null);
        Assert.That(reloadedPayLine.PlanRegistration, Is.Not.Null);
        Assert.That(reloadedPayLine.PlanRegistration.Id, Is.EqualTo(planRegistration.Id));
        Assert.That(reloadedPayLine.PlanRegistration.PlanText, Is.EqualTo("Test Plan"));
    }

    [Test]
    public async Task PlanRegistrationPayLine_MultiplePerRegistration_DoesCreate()
    {
        // Arrange
        var planRegistration = new PlanRegistration
        {
            SdkSitId = 1,
            Date = DateTime.UtcNow.Date,
            PlanText = "Sunday Shift",
            PlanHours = 14.0,
            PlanHoursInSeconds = 50400
        };
        await planRegistration.Create(DbContext).ConfigureAwait(false);

        var payLine1 = new PlanRegistrationPayLine
        {
            PlanRegistrationId = planRegistration.Id,
            PayCode = "SUN_80",
            Hours = 11.0,
            HoursInSeconds = 39600,
            CalculatedAt = DateTime.UtcNow
        };
        await payLine1.Create(DbContext).ConfigureAwait(false);

        var payLine2 = new PlanRegistrationPayLine
        {
            PlanRegistrationId = planRegistration.Id,
            PayCode = "SUN_100",
            Hours = 3.0,
            HoursInSeconds = 10800,
            CalculatedAt = DateTime.UtcNow
        };
        await payLine2.Create(DbContext).ConfigureAwait(false);

        // Act
        var payLines = await DbContext.PlanRegistrationPayLines
            .Where(p => p.PlanRegistrationId == planRegistration.Id)
            .OrderBy(p => p.PayCode)
            .ToListAsync()
            .ConfigureAwait(false);

        // Assert
        Assert.That(payLines, Is.Not.Null);
        Assert.That(payLines.Count, Is.EqualTo(2));
        Assert.That(payLines[0].PayCode, Is.EqualTo("SUN_100"));
        Assert.That(payLines[0].Hours, Is.EqualTo(3.0));
        Assert.That(payLines[1].PayCode, Is.EqualTo("SUN_80"));
        Assert.That(payLines[1].Hours, Is.EqualTo(11.0));
    }

    [Test]
    public async Task PlanRegistrationPayLine_Update_DoesUpdate()
    {
        // Arrange
        var planRegistration = new PlanRegistration
        {
            SdkSitId = 1,
            Date = DateTime.UtcNow.Date,
            PlanText = "Test Plan",
            PlanHours = 8.0,
            PlanHoursInSeconds = 28800
        };
        await planRegistration.Create(DbContext).ConfigureAwait(false);

        var payLine = new PlanRegistrationPayLine
        {
            PlanRegistrationId = planRegistration.Id,
            PayCode = "NORMAL",
            Hours = 8.0,
            HoursInSeconds = 28800,
            CalculatedAt = DateTime.UtcNow
        };
        await payLine.Create(DbContext).ConfigureAwait(false);

        // Act
        payLine.Hours = 9.0;
        payLine.HoursInSeconds = 32400;
        await payLine.Update(DbContext).ConfigureAwait(false);

        // Assert
        var payLines = DbContext.PlanRegistrationPayLines.AsNoTracking().ToList();
        var payLineVersions = DbContext.PlanRegistrationPayLineVersions.AsNoTracking().ToList();

        Assert.That(payLines.Count, Is.EqualTo(1));
        Assert.That(payLineVersions.Count, Is.EqualTo(2));

        Assert.That(payLines[0].Hours, Is.EqualTo(9.0));
        Assert.That(payLines[0].HoursInSeconds, Is.EqualTo(32400));
        Assert.That(payLines[0].Version, Is.EqualTo(2));
    }

    [Test]
    public async Task PlanRegistrationPayLine_Delete_DoesSetWorkflowStateToRemoved()
    {
        // Arrange
        var planRegistration = new PlanRegistration
        {
            SdkSitId = 1,
            Date = DateTime.UtcNow.Date,
            PlanText = "Test Plan",
            PlanHours = 8.0,
            PlanHoursInSeconds = 28800
        };
        await planRegistration.Create(DbContext).ConfigureAwait(false);

        var payLine = new PlanRegistrationPayLine
        {
            PlanRegistrationId = planRegistration.Id,
            PayCode = "NORMAL",
            Hours = 8.0,
            HoursInSeconds = 28800,
            CalculatedAt = DateTime.UtcNow
        };
        await payLine.Create(DbContext).ConfigureAwait(false);

        // Act
        await payLine.Delete(DbContext).ConfigureAwait(false);

        // Assert
        var payLines = DbContext.PlanRegistrationPayLines.AsNoTracking().ToList();
        var payLineVersions = DbContext.PlanRegistrationPayLineVersions.AsNoTracking().ToList();

        Assert.That(payLines.Count, Is.EqualTo(1));
        Assert.That(payLineVersions.Count, Is.EqualTo(2));

        Assert.That(payLines[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Removed));
        Assert.That(payLines[0].Version, Is.EqualTo(2));
    }
}
