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
public class AbsenceRequestDayUTest : DbTestFixture
{
    protected override void DoSetup()
    {
        // Ensure Messages are seeded in the database
        if (!DbContext.Messages.Any())
        {
            DbContext.Messages.Add(new Message(1, "Day Off", "Fridag", "Day off", "Freitag"));
            DbContext.Messages.Add(new Message(2, "Vacation", "Ferie", "Vacation", "Ferien"));
            DbContext.Messages.Add(new Message(3, "Sick", "Sygdom", "Sick", "Krank"));
            DbContext.SaveChanges();
        }
    }

    [Test]
    public async Task AbsenceRequestDay_Create_DoesCreate()
    {
        // Arrange
        var absenceRequest = new AbsenceRequest
        {
            RequestedBySdkSitId = 1,
            Status = AbsenceRequestStatus.Pending,
            DateFrom = new DateTime(2025, 6, 1),
            DateTo = new DateTime(2025, 6, 5),
            RequestComment = "Vacation request",
            RequestedAtUtc = DateTime.UtcNow
        };
        await absenceRequest.Create(DbContext).ConfigureAwait(false);

        var absenceRequestDay = new AbsenceRequestDay
        {
            AbsenceRequestId = absenceRequest.Id,
            Date = new DateTime(2025, 6, 1),
            MessageId = 1
        };

        // Act
        await absenceRequestDay.Create(DbContext).ConfigureAwait(false);

        // Assert
        var absenceRequestDays = DbContext.AbsenceRequestDays.AsNoTracking().ToList();
        var absenceRequestDayVersions = DbContext.AbsenceRequestDayVersions.AsNoTracking().ToList();

        Assert.That(absenceRequestDays, Is.Not.Null);
        Assert.That(absenceRequestDayVersions, Is.Not.Null);

        Assert.That(absenceRequestDays.Count, Is.EqualTo(1));
        Assert.That(absenceRequestDayVersions.Count, Is.EqualTo(1));

        Assert.That(absenceRequestDays[0].AbsenceRequestId, Is.EqualTo(absenceRequest.Id));
        Assert.That(absenceRequestDays[0].Date, Is.EqualTo(new DateTime(2025, 6, 1)));
        Assert.That(absenceRequestDays[0].MessageId, Is.EqualTo(1));
        Assert.That(absenceRequestDays[0].Version, Is.EqualTo(1));
        Assert.That(absenceRequestDays[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));

        Assert.That(absenceRequestDayVersions[0].AbsenceRequestDayId, Is.EqualTo(absenceRequestDays[0].Id));
        Assert.That(absenceRequestDayVersions[0].AbsenceRequestId, Is.EqualTo(absenceRequest.Id));
        Assert.That(absenceRequestDayVersions[0].Date, Is.EqualTo(new DateTime(2025, 6, 1)));
        Assert.That(absenceRequestDayVersions[0].MessageId, Is.EqualTo(1));
    }

    [Test]
    public async Task AbsenceRequestDay_Update_DoesUpdate()
    {
        // Arrange
        var absenceRequest = new AbsenceRequest
        {
            RequestedBySdkSitId = 1,
            Status = AbsenceRequestStatus.Pending,
            DateFrom = new DateTime(2025, 6, 1),
            DateTo = new DateTime(2025, 6, 5),
            RequestComment = "Vacation",
            RequestedAtUtc = DateTime.UtcNow
        };
        await absenceRequest.Create(DbContext).ConfigureAwait(false);

        var absenceRequestDay = new AbsenceRequestDay
        {
            AbsenceRequestId = absenceRequest.Id,
            Date = new DateTime(2025, 6, 1),
            MessageId = 1
        };
        await absenceRequestDay.Create(DbContext).ConfigureAwait(false);

        // Act
        absenceRequestDay.MessageId = 2;
        await absenceRequestDay.Update(DbContext).ConfigureAwait(false);

        // Assert
        var absenceRequestDays = DbContext.AbsenceRequestDays.AsNoTracking().ToList();
        var absenceRequestDayVersions = DbContext.AbsenceRequestDayVersions.AsNoTracking().ToList();

        Assert.That(absenceRequestDays, Is.Not.Null);
        Assert.That(absenceRequestDayVersions, Is.Not.Null);

        Assert.That(absenceRequestDays.Count, Is.EqualTo(1));
        Assert.That(absenceRequestDayVersions.Count, Is.EqualTo(2));

        Assert.That(absenceRequestDays[0].MessageId, Is.EqualTo(2));
        Assert.That(absenceRequestDays[0].Version, Is.EqualTo(2));
        Assert.That(absenceRequestDays[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));
    }

    [Test]
    public async Task AbsenceRequestDay_Delete_DoesSetWorkflowStateToRemoved()
    {
        // Arrange
        var absenceRequest = new AbsenceRequest
        {
            RequestedBySdkSitId = 1,
            Status = AbsenceRequestStatus.Pending,
            DateFrom = new DateTime(2025, 7, 1),
            DateTo = new DateTime(2025, 7, 3),
            RequestComment = "Test",
            RequestedAtUtc = DateTime.UtcNow
        };
        await absenceRequest.Create(DbContext).ConfigureAwait(false);

        var absenceRequestDay = new AbsenceRequestDay
        {
            AbsenceRequestId = absenceRequest.Id,
            Date = new DateTime(2025, 7, 1),
            MessageId = 1
        };
        await absenceRequestDay.Create(DbContext).ConfigureAwait(false);

        // Act
        await absenceRequestDay.Delete(DbContext).ConfigureAwait(false);

        // Assert
        var absenceRequestDays = DbContext.AbsenceRequestDays.AsNoTracking().ToList();
        var absenceRequestDayVersions = DbContext.AbsenceRequestDayVersions.AsNoTracking().ToList();

        Assert.That(absenceRequestDays, Is.Not.Null);
        Assert.That(absenceRequestDayVersions, Is.Not.Null);

        Assert.That(absenceRequestDays.Count, Is.EqualTo(1));
        Assert.That(absenceRequestDayVersions.Count, Is.EqualTo(2));

        Assert.That(absenceRequestDays[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Removed));
        Assert.That(absenceRequestDays[0].Version, Is.EqualTo(2));
    }

    [Test]
    public async Task AbsenceRequestDay_CreateMultipleDaysForOneRequest_DoesCreate()
    {
        // Arrange
        var absenceRequest = new AbsenceRequest
        {
            RequestedBySdkSitId = 1,
            Status = AbsenceRequestStatus.Approved,
            DateFrom = new DateTime(2025, 8, 1),
            DateTo = new DateTime(2025, 8, 5),
            RequestComment = "Week off",
            RequestedAtUtc = DateTime.UtcNow.AddDays(-5),
            DecidedAtUtc = DateTime.UtcNow.AddDays(-2),
            DecidedBySdkSitId = 2,
            DecisionComment = "Approved"
        };
        await absenceRequest.Create(DbContext).ConfigureAwait(false);

        var day1 = new AbsenceRequestDay
        {
            AbsenceRequestId = absenceRequest.Id,
            Date = new DateTime(2025, 8, 1),
            MessageId = 1
        };
        var day2 = new AbsenceRequestDay
        {
            AbsenceRequestId = absenceRequest.Id,
            Date = new DateTime(2025, 8, 2),
            MessageId = 1
        };
        var day3 = new AbsenceRequestDay
        {
            AbsenceRequestId = absenceRequest.Id,
            Date = new DateTime(2025, 8, 5),
            MessageId = 2
        };

        // Act
        await day1.Create(DbContext).ConfigureAwait(false);
        await day2.Create(DbContext).ConfigureAwait(false);
        await day3.Create(DbContext).ConfigureAwait(false);

        // Assert
        var absenceRequestDays = DbContext.AbsenceRequestDays.AsNoTracking().ToList();

        Assert.That(absenceRequestDays, Is.Not.Null);
        Assert.That(absenceRequestDays.Count, Is.EqualTo(3));

        Assert.That(absenceRequestDays.All(d => d.AbsenceRequestId == absenceRequest.Id), Is.True);
        Assert.That(absenceRequestDays.Select(d => d.Date).ToList(), 
            Is.EquivalentTo(new[] { new DateTime(2025, 8, 1), new DateTime(2025, 8, 2), new DateTime(2025, 8, 5) }));
    }
}
