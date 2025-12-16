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
    public class GpsCoordinateUTest : DbTestFixture
    {
        [Test]
        public async Task GpsCoordinate_Save_DoesSave()
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

            var gpsCoordinate = new GpsCoordinate
            {
                PlanRegistrationId = planRegistration.Id,
                Latitude = 55.6761,
                Longitude = 12.5683,
                RegistrationType = "Start1StartedAt",
                CreatedByUserId = 1,
                UpdatedByUserId = 1,
            };

            // Act
            await gpsCoordinate.Create(DbContext);

            var gpsCoordinateList = DbContext.GpsCoordinates
                .AsNoTracking()
                .ToList();

            var gpsCoordinateVersionsList = DbContext.GpsCoordinateVersions.AsNoTracking().ToList();

            // Assert
            Assert.That(gpsCoordinateList.Count, Is.EqualTo(1));
            Assert.That(gpsCoordinateVersionsList.Count, Is.EqualTo(1));
            Assert.That(gpsCoordinateList[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));
            Assert.That(gpsCoordinateList[0].CreatedByUserId, Is.EqualTo(gpsCoordinate.CreatedByUserId));
            Assert.That(gpsCoordinateList[0].UpdatedByUserId, Is.EqualTo(gpsCoordinate.UpdatedByUserId));
            Assert.That(gpsCoordinateList[0].PlanRegistrationId, Is.EqualTo(gpsCoordinate.PlanRegistrationId));
            Assert.That(gpsCoordinateList[0].Latitude, Is.EqualTo(gpsCoordinate.Latitude));
            Assert.That(gpsCoordinateList[0].Longitude, Is.EqualTo(gpsCoordinate.Longitude));
            Assert.That(gpsCoordinateList[0].RegistrationType, Is.EqualTo(gpsCoordinate.RegistrationType));
            Assert.That(gpsCoordinateList[0].Id, Is.EqualTo(gpsCoordinate.Id));
            Assert.That(gpsCoordinateList[0].Version, Is.EqualTo(1));

            // versions
            Assert.That(gpsCoordinateVersionsList[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));
            Assert.That(gpsCoordinateVersionsList[0].CreatedByUserId, Is.EqualTo(gpsCoordinate.CreatedByUserId));
            Assert.That(gpsCoordinateVersionsList[0].UpdatedByUserId, Is.EqualTo(gpsCoordinate.UpdatedByUserId));
            Assert.That(gpsCoordinateVersionsList[0].PlanRegistrationId, Is.EqualTo(gpsCoordinate.PlanRegistrationId));
            Assert.That(gpsCoordinateVersionsList[0].Latitude, Is.EqualTo(gpsCoordinate.Latitude));
            Assert.That(gpsCoordinateVersionsList[0].Longitude, Is.EqualTo(gpsCoordinate.Longitude));
            Assert.That(gpsCoordinateVersionsList[0].RegistrationType, Is.EqualTo(gpsCoordinate.RegistrationType));
            Assert.That(gpsCoordinateVersionsList[0].GpsCoordinateId, Is.EqualTo(gpsCoordinate.Id));
            Assert.That(gpsCoordinateVersionsList[0].Version, Is.EqualTo(1));
        }

        [Test]
        public async Task GpsCoordinate_Update_DoesUpdate()
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

            var gpsCoordinate = new GpsCoordinate
            {
                PlanRegistrationId = planRegistration.Id,
                Latitude = 55.6761,
                Longitude = 12.5683,
                RegistrationType = "Start1StartedAt",
                CreatedByUserId = 1,
                UpdatedByUserId = 1,
            };
            await gpsCoordinate.Create(DbContext);

            // Act
            var gpsCoordinateOld = await DbContext.GpsCoordinates.AsNoTracking().FirstOrDefaultAsync();

            gpsCoordinate.Latitude = 55.1234;
            gpsCoordinate.Longitude = 12.9876;
            gpsCoordinate.RegistrationType = "Stop1StoppedAt";
            await gpsCoordinate.Update(DbContext);

            var gpsCoordinateList = DbContext.GpsCoordinates.AsNoTracking().ToList();
            var gpsCoordinateVersionsList = DbContext.GpsCoordinateVersions.AsNoTracking().ToList();

            // Assert
            Assert.That(gpsCoordinateList.Count, Is.EqualTo(1));
            Assert.That(gpsCoordinateVersionsList.Count, Is.EqualTo(2));
            Assert.That(gpsCoordinateList[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));
            Assert.That(gpsCoordinateList[0].Latitude, Is.EqualTo(gpsCoordinate.Latitude));
            Assert.That(gpsCoordinateList[0].Longitude, Is.EqualTo(gpsCoordinate.Longitude));
            Assert.That(gpsCoordinateList[0].RegistrationType, Is.EqualTo(gpsCoordinate.RegistrationType));
            Assert.That(gpsCoordinateList[0].Version, Is.EqualTo(2));

            // versions
            Assert.That(gpsCoordinateVersionsList[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));
            Assert.That(gpsCoordinateVersionsList[0].Latitude, Is.EqualTo(gpsCoordinateOld.Latitude));
            Assert.That(gpsCoordinateVersionsList[0].Longitude, Is.EqualTo(gpsCoordinateOld.Longitude));
            Assert.That(gpsCoordinateVersionsList[0].RegistrationType, Is.EqualTo(gpsCoordinateOld.RegistrationType));
            Assert.That(gpsCoordinateVersionsList[0].Version, Is.EqualTo(1));

            Assert.That(gpsCoordinateVersionsList[1].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));
            Assert.That(gpsCoordinateVersionsList[1].Latitude, Is.EqualTo(gpsCoordinate.Latitude));
            Assert.That(gpsCoordinateVersionsList[1].Longitude, Is.EqualTo(gpsCoordinate.Longitude));
            Assert.That(gpsCoordinateVersionsList[1].RegistrationType, Is.EqualTo(gpsCoordinate.RegistrationType));
            Assert.That(gpsCoordinateVersionsList[1].Version, Is.EqualTo(2));
        }

        [Test]
        public async Task GpsCoordinate_Delete_DoesDelete()
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

            var gpsCoordinate = new GpsCoordinate
            {
                PlanRegistrationId = planRegistration.Id,
                Latitude = 55.6761,
                Longitude = 12.5683,
                RegistrationType = "Start1StartedAt",
                CreatedByUserId = 1,
                UpdatedByUserId = 1,
            };
            await gpsCoordinate.Create(DbContext);

            // Act
            var gpsCoordinateOld = await DbContext.GpsCoordinates.AsNoTracking().FirstOrDefaultAsync();
            await gpsCoordinate.Delete(DbContext);

            var gpsCoordinateList = DbContext.GpsCoordinates.AsNoTracking().ToList();
            var gpsCoordinateVersionsList = DbContext.GpsCoordinateVersions.AsNoTracking().ToList();

            // Assert
            Assert.That(gpsCoordinateList.Count, Is.EqualTo(1));
            Assert.That(gpsCoordinateVersionsList.Count, Is.EqualTo(2));
            Assert.That(gpsCoordinateList[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Removed));
            Assert.That(gpsCoordinateList[0].Version, Is.EqualTo(2));

            // versions
            Assert.That(gpsCoordinateVersionsList[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));
            Assert.That(gpsCoordinateVersionsList[0].Version, Is.EqualTo(1));

            Assert.That(gpsCoordinateVersionsList[1].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Removed));
            Assert.That(gpsCoordinateVersionsList[1].Version, Is.EqualTo(2));
        }
    }
}
