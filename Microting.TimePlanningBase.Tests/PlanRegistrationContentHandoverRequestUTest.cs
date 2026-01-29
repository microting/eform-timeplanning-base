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
public class PlanRegistrationContentHandoverRequestUTest : DbTestFixture
{
    [Test]
    public async Task PlanRegistrationContentHandoverRequest_Create_DoesCreate()
    {
        // Arrange
        var request = new PlanRegistrationContentHandoverRequest
        {
            Date = new DateTime(2025, 12, 25),
            FromSdkSitId = 1,
            ToSdkSitId = 2,
            FromPlanRegistrationId = 100,
            ToPlanRegistrationId = 200,
            Status = HandoverRequestStatus.Pending,
            RequestedByComment = "Please take over my shift",
            DecisionComment = null,
            RequestedAtUtc = new DateTime(2025, 12, 20, 10, 0, 0, DateTimeKind.Utc),
            RespondedAtUtc = null
        };

        // Act
        await request.Create(DbContext).ConfigureAwait(false);

        // Assert
        var requests = DbContext.PlanRegistrationContentHandoverRequests.AsNoTracking().ToList();
        var requestVersions = DbContext.PlanRegistrationContentHandoverRequestVersions.AsNoTracking().ToList();

        Assert.That(requests, Is.Not.Null);
        Assert.That(requestVersions, Is.Not.Null);

        Assert.That(requests.Count, Is.EqualTo(1));
        Assert.That(requestVersions.Count, Is.EqualTo(1));

        Assert.That(requests[0].Date, Is.EqualTo(new DateTime(2025, 12, 25)));
        Assert.That(requests[0].FromSdkSitId, Is.EqualTo(1));
        Assert.That(requests[0].ToSdkSitId, Is.EqualTo(2));
        Assert.That(requests[0].FromPlanRegistrationId, Is.EqualTo(100));
        Assert.That(requests[0].ToPlanRegistrationId, Is.EqualTo(200));
        Assert.That(requests[0].Status, Is.EqualTo(HandoverRequestStatus.Pending));
        Assert.That(requests[0].RequestedByComment, Is.EqualTo("Please take over my shift"));
        Assert.That(requests[0].DecisionComment, Is.Null);
        Assert.That(requests[0].RequestedAtUtc, Is.EqualTo(new DateTime(2025, 12, 20, 10, 0, 0, DateTimeKind.Utc)));
        Assert.That(requests[0].RespondedAtUtc, Is.Null);
        Assert.That(requests[0].Version, Is.EqualTo(1));
        Assert.That(requests[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));

        Assert.That(requestVersions[0].Date, Is.EqualTo(new DateTime(2025, 12, 25)));
        Assert.That(requestVersions[0].FromSdkSitId, Is.EqualTo(1));
        Assert.That(requestVersions[0].ToSdkSitId, Is.EqualTo(2));
        Assert.That(requestVersions[0].FromPlanRegistrationId, Is.EqualTo(100));
        Assert.That(requestVersions[0].ToPlanRegistrationId, Is.EqualTo(200));
        Assert.That(requestVersions[0].Status, Is.EqualTo(HandoverRequestStatus.Pending));
        Assert.That(requestVersions[0].RequestedByComment, Is.EqualTo("Please take over my shift"));
        Assert.That(requestVersions[0].PlanRegistrationContentHandoverRequestId, Is.EqualTo(requests[0].Id));
    }

    [Test]
    public async Task PlanRegistrationContentHandoverRequest_Update_DoesUpdate()
    {
        // Arrange
        var request = new PlanRegistrationContentHandoverRequest
        {
            Date = new DateTime(2025, 12, 25),
            FromSdkSitId = 1,
            ToSdkSitId = 2,
            FromPlanRegistrationId = 100,
            ToPlanRegistrationId = 200,
            Status = HandoverRequestStatus.Pending,
            RequestedByComment = "Please take over my shift",
            RequestedAtUtc = new DateTime(2025, 12, 20, 10, 0, 0, DateTimeKind.Utc)
        };
        await request.Create(DbContext).ConfigureAwait(false);

        // Act
        request.Status = HandoverRequestStatus.Accepted;
        request.DecisionComment = "Accepted your request";
        request.RespondedAtUtc = new DateTime(2025, 12, 21, 14, 30, 0, DateTimeKind.Utc);
        await request.Update(DbContext).ConfigureAwait(false);

        // Assert
        var requests = DbContext.PlanRegistrationContentHandoverRequests.AsNoTracking().ToList();
        var requestVersions = DbContext.PlanRegistrationContentHandoverRequestVersions.AsNoTracking().ToList();

        Assert.That(requests, Is.Not.Null);
        Assert.That(requestVersions, Is.Not.Null);

        Assert.That(requests.Count, Is.EqualTo(1));
        Assert.That(requestVersions.Count, Is.EqualTo(2));

        Assert.That(requests[0].Status, Is.EqualTo(HandoverRequestStatus.Accepted));
        Assert.That(requests[0].DecisionComment, Is.EqualTo("Accepted your request"));
        Assert.That(requests[0].RespondedAtUtc, Is.EqualTo(new DateTime(2025, 12, 21, 14, 30, 0, DateTimeKind.Utc)));
        Assert.That(requests[0].Version, Is.EqualTo(2));
        Assert.That(requests[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));
    }

    [Test]
    public async Task PlanRegistrationContentHandoverRequest_Delete_DoesSetWorkflowStateToRemoved()
    {
        // Arrange
        var request = new PlanRegistrationContentHandoverRequest
        {
            Date = new DateTime(2025, 12, 25),
            FromSdkSitId = 1,
            ToSdkSitId = 2,
            FromPlanRegistrationId = 100,
            ToPlanRegistrationId = 200,
            Status = HandoverRequestStatus.Pending,
            RequestedByComment = "Please take over my shift",
            RequestedAtUtc = new DateTime(2025, 12, 20, 10, 0, 0, DateTimeKind.Utc)
        };
        await request.Create(DbContext).ConfigureAwait(false);

        // Act
        await request.Delete(DbContext).ConfigureAwait(false);

        // Assert
        var requests = DbContext.PlanRegistrationContentHandoverRequests.AsNoTracking().ToList();
        var requestVersions = DbContext.PlanRegistrationContentHandoverRequestVersions.AsNoTracking().ToList();

        Assert.That(requests, Is.Not.Null);
        Assert.That(requestVersions, Is.Not.Null);

        Assert.That(requests.Count, Is.EqualTo(1));
        Assert.That(requestVersions.Count, Is.EqualTo(2));

        Assert.That(requests[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Removed));
        Assert.That(requests[0].Status, Is.EqualTo(HandoverRequestStatus.Pending));
        Assert.That(requests[0].Version, Is.EqualTo(2));
    }

    [Test]
    public async Task PlanRegistrationContentHandoverRequest_CreateWithDefaultStatus_DoesCreate()
    {
        // Arrange
        var request = new PlanRegistrationContentHandoverRequest
        {
            Date = new DateTime(2025, 12, 25),
            FromSdkSitId = 1,
            ToSdkSitId = 2,
            FromPlanRegistrationId = 100,
            ToPlanRegistrationId = 200,
            RequestedByComment = "Please take over my shift"
        };

        // Act
        await request.Create(DbContext).ConfigureAwait(false);

        // Assert
        var requests = DbContext.PlanRegistrationContentHandoverRequests.AsNoTracking().ToList();

        Assert.That(requests, Is.Not.Null);
        Assert.That(requests.Count, Is.EqualTo(1));
        Assert.That(requests[0].Status, Is.EqualTo(HandoverRequestStatus.Pending));
    }

    [Test]
    public async Task PlanRegistrationContentHandoverRequest_UpdateToRejected_DoesUpdate()
    {
        // Arrange
        var request = new PlanRegistrationContentHandoverRequest
        {
            Date = new DateTime(2025, 12, 25),
            FromSdkSitId = 1,
            ToSdkSitId = 2,
            FromPlanRegistrationId = 100,
            ToPlanRegistrationId = 200,
            Status = HandoverRequestStatus.Pending,
            RequestedByComment = "Please take over my shift",
            RequestedAtUtc = new DateTime(2025, 12, 20, 10, 0, 0, DateTimeKind.Utc)
        };
        await request.Create(DbContext).ConfigureAwait(false);

        // Act
        request.Status = HandoverRequestStatus.Rejected;
        request.DecisionComment = "Sorry, I cannot take this shift";
        request.RespondedAtUtc = new DateTime(2025, 12, 21, 15, 0, 0, DateTimeKind.Utc);
        await request.Update(DbContext).ConfigureAwait(false);

        // Assert
        var requests = DbContext.PlanRegistrationContentHandoverRequests.AsNoTracking().ToList();

        Assert.That(requests, Is.Not.Null);
        Assert.That(requests.Count, Is.EqualTo(1));
        Assert.That(requests[0].Status, Is.EqualTo(HandoverRequestStatus.Rejected));
        Assert.That(requests[0].DecisionComment, Is.EqualTo("Sorry, I cannot take this shift"));
    }

    [Test]
    public async Task PlanRegistrationContentHandoverRequest_UpdateToCancelled_DoesUpdate()
    {
        // Arrange
        var request = new PlanRegistrationContentHandoverRequest
        {
            Date = new DateTime(2025, 12, 25),
            FromSdkSitId = 1,
            ToSdkSitId = 2,
            FromPlanRegistrationId = 100,
            ToPlanRegistrationId = 200,
            Status = HandoverRequestStatus.Pending,
            RequestedByComment = "Please take over my shift",
            RequestedAtUtc = new DateTime(2025, 12, 20, 10, 0, 0, DateTimeKind.Utc)
        };
        await request.Create(DbContext).ConfigureAwait(false);

        // Act
        request.Status = HandoverRequestStatus.Cancelled;
        await request.Update(DbContext).ConfigureAwait(false);

        // Assert
        var requests = DbContext.PlanRegistrationContentHandoverRequests.AsNoTracking().ToList();

        Assert.That(requests, Is.Not.Null);
        Assert.That(requests.Count, Is.EqualTo(1));
        Assert.That(requests[0].Status, Is.EqualTo(HandoverRequestStatus.Cancelled));
    }

    [Test]
    public async Task PlanRegistrationContentHandoverRequest_UpdateToExpired_DoesUpdate()
    {
        // Arrange
        var request = new PlanRegistrationContentHandoverRequest
        {
            Date = new DateTime(2025, 12, 25),
            FromSdkSitId = 1,
            ToSdkSitId = 2,
            FromPlanRegistrationId = 100,
            ToPlanRegistrationId = 200,
            Status = HandoverRequestStatus.Pending,
            RequestedByComment = "Please take over my shift",
            RequestedAtUtc = new DateTime(2025, 12, 20, 10, 0, 0, DateTimeKind.Utc)
        };
        await request.Create(DbContext).ConfigureAwait(false);

        // Act
        request.Status = HandoverRequestStatus.Expired;
        await request.Update(DbContext).ConfigureAwait(false);

        // Assert
        var requests = DbContext.PlanRegistrationContentHandoverRequests.AsNoTracking().ToList();

        Assert.That(requests, Is.Not.Null);
        Assert.That(requests.Count, Is.EqualTo(1));
        Assert.That(requests[0].Status, Is.EqualTo(HandoverRequestStatus.Expired));
    }
}
