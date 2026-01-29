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
public class AbsenceRequestUTest : DbTestFixture
{
    [Test]
    public async Task AbsenceRequest_Create_DoesCreate()
    {
        // Arrange
        var absenceRequest = new AbsenceRequest
        {
            RequestedBySdkSitId = 1,
            Status = AbsenceRequestStatus.Pending,
            DateFrom = new DateTime(2025, 6, 1),
            DateTo = new DateTime(2025, 6, 5),
            RequestComment = "Summer vacation request",
            RequestedAtUtc = DateTime.UtcNow
        };

        // Act
        await absenceRequest.Create(DbContext).ConfigureAwait(false);

        // Assert
        var absenceRequests = DbContext.AbsenceRequests.AsNoTracking().ToList();
        var absenceRequestVersions = DbContext.AbsenceRequestVersions.AsNoTracking().ToList();

        Assert.That(absenceRequests, Is.Not.Null);
        Assert.That(absenceRequestVersions, Is.Not.Null);

        Assert.That(absenceRequests.Count, Is.EqualTo(1));
        Assert.That(absenceRequestVersions.Count, Is.EqualTo(1));

        Assert.That(absenceRequests[0].RequestedBySdkSitId, Is.EqualTo(1));
        Assert.That(absenceRequests[0].Status, Is.EqualTo(AbsenceRequestStatus.Pending));
        Assert.That(absenceRequests[0].DateFrom, Is.EqualTo(new DateTime(2025, 6, 1)));
        Assert.That(absenceRequests[0].DateTo, Is.EqualTo(new DateTime(2025, 6, 5)));
        Assert.That(absenceRequests[0].RequestComment, Is.EqualTo("Summer vacation request"));
        Assert.That(absenceRequests[0].DecidedBySdkSitId, Is.Null);
        Assert.That(absenceRequests[0].DecisionComment, Is.Null);
        Assert.That(absenceRequests[0].DecidedAtUtc, Is.Null);
        Assert.That(absenceRequests[0].Version, Is.EqualTo(1));
        Assert.That(absenceRequests[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));

        Assert.That(absenceRequestVersions[0].AbsenceRequestId, Is.EqualTo(absenceRequests[0].Id));
        Assert.That(absenceRequestVersions[0].RequestedBySdkSitId, Is.EqualTo(1));
        Assert.That(absenceRequestVersions[0].Status, Is.EqualTo(AbsenceRequestStatus.Pending));
    }

    [Test]
    public async Task AbsenceRequest_Update_DoesUpdate()
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

        // Act
        absenceRequest.Status = AbsenceRequestStatus.Approved;
        absenceRequest.DecidedBySdkSitId = 2;
        absenceRequest.DecisionComment = "Approved";
        absenceRequest.DecidedAtUtc = DateTime.UtcNow;
        await absenceRequest.Update(DbContext).ConfigureAwait(false);

        // Assert
        var absenceRequests = DbContext.AbsenceRequests.AsNoTracking().ToList();
        var absenceRequestVersions = DbContext.AbsenceRequestVersions.AsNoTracking().ToList();

        Assert.That(absenceRequests, Is.Not.Null);
        Assert.That(absenceRequestVersions, Is.Not.Null);

        Assert.That(absenceRequests.Count, Is.EqualTo(1));
        Assert.That(absenceRequestVersions.Count, Is.EqualTo(2));

        Assert.That(absenceRequests[0].Status, Is.EqualTo(AbsenceRequestStatus.Approved));
        Assert.That(absenceRequests[0].DecidedBySdkSitId, Is.EqualTo(2));
        Assert.That(absenceRequests[0].DecisionComment, Is.EqualTo("Approved"));
        Assert.That(absenceRequests[0].DecidedAtUtc, Is.Not.Null);
        Assert.That(absenceRequests[0].Version, Is.EqualTo(2));
        Assert.That(absenceRequests[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));
    }

    [Test]
    public async Task AbsenceRequest_Delete_DoesSetWorkflowStateToRemoved()
    {
        // Arrange
        var absenceRequest = new AbsenceRequest
        {
            RequestedBySdkSitId = 1,
            Status = AbsenceRequestStatus.Pending,
            DateFrom = new DateTime(2025, 7, 1),
            DateTo = new DateTime(2025, 7, 3),
            RequestComment = "Test request",
            RequestedAtUtc = DateTime.UtcNow
        };
        await absenceRequest.Create(DbContext).ConfigureAwait(false);

        // Act
        await absenceRequest.Delete(DbContext).ConfigureAwait(false);

        // Assert
        var absenceRequests = DbContext.AbsenceRequests.AsNoTracking().ToList();
        var absenceRequestVersions = DbContext.AbsenceRequestVersions.AsNoTracking().ToList();

        Assert.That(absenceRequests, Is.Not.Null);
        Assert.That(absenceRequestVersions, Is.Not.Null);

        Assert.That(absenceRequests.Count, Is.EqualTo(1));
        Assert.That(absenceRequestVersions.Count, Is.EqualTo(2));

        Assert.That(absenceRequests[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Removed));
        Assert.That(absenceRequests[0].RequestComment, Is.EqualTo("Test request"));
        Assert.That(absenceRequests[0].Version, Is.EqualTo(2));
    }

    [Test]
    public async Task AbsenceRequest_CreateWithRejectedStatus_DoesCreate()
    {
        // Arrange
        var absenceRequest = new AbsenceRequest
        {
            RequestedBySdkSitId = 1,
            DecidedBySdkSitId = 2,
            Status = AbsenceRequestStatus.Rejected,
            DateFrom = new DateTime(2025, 8, 1),
            DateTo = new DateTime(2025, 8, 3),
            RequestComment = "Leave request",
            DecisionComment = "Not approved due to staff shortage",
            RequestedAtUtc = DateTime.UtcNow.AddDays(-1),
            DecidedAtUtc = DateTime.UtcNow
        };

        // Act
        await absenceRequest.Create(DbContext).ConfigureAwait(false);

        // Assert
        var absenceRequests = DbContext.AbsenceRequests.AsNoTracking().ToList();

        Assert.That(absenceRequests, Is.Not.Null);
        Assert.That(absenceRequests.Count, Is.EqualTo(1));
        Assert.That(absenceRequests[0].Status, Is.EqualTo(AbsenceRequestStatus.Rejected));
        Assert.That(absenceRequests[0].DecisionComment, Is.EqualTo("Not approved due to staff shortage"));
        Assert.That(absenceRequests[0].DecidedBySdkSitId, Is.EqualTo(2));
    }

    [Test]
    public async Task AbsenceRequest_CreateWithCancelledStatus_DoesCreate()
    {
        // Arrange
        var absenceRequest = new AbsenceRequest
        {
            RequestedBySdkSitId = 1,
            Status = AbsenceRequestStatus.Cancelled,
            DateFrom = new DateTime(2025, 9, 1),
            DateTo = new DateTime(2025, 9, 2),
            RequestComment = "Need time off",
            DecisionComment = "Cancelled by employee",
            RequestedAtUtc = DateTime.UtcNow.AddDays(-2),
            DecidedAtUtc = DateTime.UtcNow
        };

        // Act
        await absenceRequest.Create(DbContext).ConfigureAwait(false);

        // Assert
        var absenceRequests = DbContext.AbsenceRequests.AsNoTracking().ToList();

        Assert.That(absenceRequests, Is.Not.Null);
        Assert.That(absenceRequests.Count, Is.EqualTo(1));
        Assert.That(absenceRequests[0].Status, Is.EqualTo(AbsenceRequestStatus.Cancelled));
    }
}
