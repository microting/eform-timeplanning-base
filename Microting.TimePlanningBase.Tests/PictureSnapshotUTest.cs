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

namespace Microting.TimePlanningBase.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using eForm.Infrastructure.Constants;
    using Infrastructure.Data.Entities;
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;

    [TestFixture]
    public class PictureSnapshotUTest : DbTestFixture
    {
        [Test]
        public async Task PictureSnapshot_Save_DoesSave()
        {
            // Arrange
            var assignedSite = new AssignedSite
            {
                SiteId = 1,
            };
            await assignedSite.Create(DbContext);

            var planRegistration = new PlanRegistration
            {
                SdkSitId = 1,
                Date = DateTime.Now,
                UpdatedByUserId = 1,
                CreatedByUserId = 1,
            };
            await planRegistration.Create(DbContext);

            var pictureSnapshot = new PictureSnapshot
            {
                PlanRegistrationId = planRegistration.Id,
                PictureHash = "abc123def456hash",
                RegistrationType = "Start1StartedAt",
                CreatedByUserId = 1,
                UpdatedByUserId = 1,
            };

            // Act
            await pictureSnapshot.Create(DbContext);

            var pictureSnapshotList = DbContext.PictureSnapshots
                .AsNoTracking()
                .ToList();

            var pictureSnapshotVersionsList = DbContext.PictureSnapshotVersions.AsNoTracking().ToList();

            // Assert
            Assert.That(pictureSnapshotList.Count, Is.EqualTo(1));
            Assert.That(pictureSnapshotVersionsList.Count, Is.EqualTo(1));
            Assert.That(pictureSnapshotList[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));
            Assert.That(pictureSnapshotList[0].CreatedByUserId, Is.EqualTo(pictureSnapshot.CreatedByUserId));
            Assert.That(pictureSnapshotList[0].UpdatedByUserId, Is.EqualTo(pictureSnapshot.UpdatedByUserId));
            Assert.That(pictureSnapshotList[0].PlanRegistrationId, Is.EqualTo(pictureSnapshot.PlanRegistrationId));
            Assert.That(pictureSnapshotList[0].PictureHash, Is.EqualTo(pictureSnapshot.PictureHash));
            Assert.That(pictureSnapshotList[0].RegistrationType, Is.EqualTo(pictureSnapshot.RegistrationType));
            Assert.That(pictureSnapshotList[0].Id, Is.EqualTo(pictureSnapshot.Id));
            Assert.That(pictureSnapshotList[0].Version, Is.EqualTo(1));

            // versions
            Assert.That(pictureSnapshotVersionsList[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));
            Assert.That(pictureSnapshotVersionsList[0].CreatedByUserId, Is.EqualTo(pictureSnapshot.CreatedByUserId));
            Assert.That(pictureSnapshotVersionsList[0].UpdatedByUserId, Is.EqualTo(pictureSnapshot.UpdatedByUserId));
            Assert.That(pictureSnapshotVersionsList[0].PlanRegistrationId, Is.EqualTo(pictureSnapshot.PlanRegistrationId));
            Assert.That(pictureSnapshotVersionsList[0].PictureHash, Is.EqualTo(pictureSnapshot.PictureHash));
            Assert.That(pictureSnapshotVersionsList[0].RegistrationType, Is.EqualTo(pictureSnapshot.RegistrationType));
            Assert.That(pictureSnapshotVersionsList[0].PictureSnapshotId, Is.EqualTo(pictureSnapshot.Id));
            Assert.That(pictureSnapshotVersionsList[0].Version, Is.EqualTo(1));
        }

        [Test]
        public async Task PictureSnapshot_Update_DoesUpdate()
        {
            // Arrange
            var assignedSite = new AssignedSite
            {
                SiteId = 1,
            };
            await assignedSite.Create(DbContext);

            var planRegistration = new PlanRegistration
            {
                SdkSitId = 1,
                Date = DateTime.Now,
                UpdatedByUserId = 1,
                CreatedByUserId = 1,
            };
            await planRegistration.Create(DbContext);

            var pictureSnapshot = new PictureSnapshot
            {
                PlanRegistrationId = planRegistration.Id,
                PictureHash = "abc123def456hash",
                RegistrationType = "Start1StartedAt",
                CreatedByUserId = 1,
                UpdatedByUserId = 1,
            };
            await pictureSnapshot.Create(DbContext);

            // Act
            var pictureSnapshotOld = await DbContext.PictureSnapshots.AsNoTracking().FirstOrDefaultAsync();

            pictureSnapshot.PictureHash = "xyz789newhash";
            pictureSnapshot.RegistrationType = "Stop1StoppedAt";
            await pictureSnapshot.Update(DbContext);

            var pictureSnapshotList = DbContext.PictureSnapshots.AsNoTracking().ToList();
            var pictureSnapshotVersionsList = DbContext.PictureSnapshotVersions.AsNoTracking().ToList();

            // Assert
            Assert.That(pictureSnapshotList.Count, Is.EqualTo(1));
            Assert.That(pictureSnapshotVersionsList.Count, Is.EqualTo(2));
            Assert.That(pictureSnapshotList[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));
            Assert.That(pictureSnapshotList[0].PictureHash, Is.EqualTo(pictureSnapshot.PictureHash));
            Assert.That(pictureSnapshotList[0].RegistrationType, Is.EqualTo(pictureSnapshot.RegistrationType));
            Assert.That(pictureSnapshotList[0].Version, Is.EqualTo(2));

            // versions
            Assert.That(pictureSnapshotVersionsList[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));
            Assert.That(pictureSnapshotVersionsList[0].PictureHash, Is.EqualTo(pictureSnapshotOld.PictureHash));
            Assert.That(pictureSnapshotVersionsList[0].RegistrationType, Is.EqualTo(pictureSnapshotOld.RegistrationType));
            Assert.That(pictureSnapshotVersionsList[0].Version, Is.EqualTo(1));

            Assert.That(pictureSnapshotVersionsList[1].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));
            Assert.That(pictureSnapshotVersionsList[1].PictureHash, Is.EqualTo(pictureSnapshot.PictureHash));
            Assert.That(pictureSnapshotVersionsList[1].RegistrationType, Is.EqualTo(pictureSnapshot.RegistrationType));
            Assert.That(pictureSnapshotVersionsList[1].Version, Is.EqualTo(2));
        }

        [Test]
        public async Task PictureSnapshot_Delete_DoesDelete()
        {
            // Arrange
            var assignedSite = new AssignedSite
            {
                SiteId = 1,
            };
            await assignedSite.Create(DbContext);

            var planRegistration = new PlanRegistration
            {
                SdkSitId = 1,
                Date = DateTime.Now,
                UpdatedByUserId = 1,
                CreatedByUserId = 1,
            };
            await planRegistration.Create(DbContext);

            var pictureSnapshot = new PictureSnapshot
            {
                PlanRegistrationId = planRegistration.Id,
                PictureHash = "abc123def456hash",
                RegistrationType = "Start1StartedAt",
                CreatedByUserId = 1,
                UpdatedByUserId = 1,
            };
            await pictureSnapshot.Create(DbContext);

            // Act
            var pictureSnapshotOld = await DbContext.PictureSnapshots.AsNoTracking().FirstOrDefaultAsync();
            await pictureSnapshot.Delete(DbContext);

            var pictureSnapshotList = DbContext.PictureSnapshots.AsNoTracking().ToList();
            var pictureSnapshotVersionsList = DbContext.PictureSnapshotVersions.AsNoTracking().ToList();

            // Assert
            Assert.That(pictureSnapshotList.Count, Is.EqualTo(1));
            Assert.That(pictureSnapshotVersionsList.Count, Is.EqualTo(2));
            Assert.That(pictureSnapshotList[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Removed));
            Assert.That(pictureSnapshotList[0].Version, Is.EqualTo(2));

            // versions
            Assert.That(pictureSnapshotVersionsList[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));
            Assert.That(pictureSnapshotVersionsList[0].Version, Is.EqualTo(1));

            Assert.That(pictureSnapshotVersionsList[1].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Removed));
            Assert.That(pictureSnapshotVersionsList[1].Version, Is.EqualTo(2));
        }
    }
}
