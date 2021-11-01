/*
The MIT License (MIT)

Copyright (c) 2007 - 2019 Microting A/S

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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using eForm.Infrastructure.Constants;
    using eForm.Infrastructure.Data.Entities;
    using Infrastructure.Data.Entities;
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;

    [TestFixture]
    public class PlanningUTest : DbTestFixture
    {
        [Test]
        public async Task PlanRegistration_Save_DoesSave()
        {
            // Arrange

            var assignedSite = new AssignedSite
            {
                SiteId = 1,
            };
            await assignedSite.Create(DbContext);
            var planRegistration = new PlanRegistration
            {
                AssignedSiteId = 1,
                CommentOffice = Guid.NewGuid().ToString(),
                CommentOfficeAll = Guid.NewGuid().ToString(),
                Flex = new Random().NextDouble(),
                Date = DateTime.Now,
                UpdatedByUserId = 1,
                CreatedByUserId = 1,
                MessageId = new Random().Next(1, 8),
                NettoHours = new Random().NextDouble(),
                PlanText = Guid.NewGuid().ToString(),
                PaiedOutFlex = new Random().NextDouble(),
                Pause1Id = new Random().Next(),
                Pause2Id = new Random().Next(),
                Start1Id = new Random().Next(),
                Start2Id = new Random().Next(),
                Stop1Id = new Random().Next(),
                Stop2Id = new Random().Next(),
                PlanHours = new Random().NextDouble(),
                SumFlex = new Random().NextDouble(),
            };

            // Act
            await planRegistration.Create(DbContext);
            
            var planRegistrationList = DbContext.PlanRegistrations
                .AsNoTracking()
                .ToList();
            

            var planRegistrationVersionsList = DbContext.PlanRegistrationVersions.AsNoTracking().ToList();
            
            // Assert
            Assert.AreEqual(1, planRegistrationList.Count);
            Assert.AreEqual(1, planRegistrationVersionsList.Count);
            Assert.AreEqual(Constants.WorkflowStates.Created, planRegistrationList[0].WorkflowState);
            Assert.AreEqual(planRegistration.CreatedByUserId, planRegistrationList[0].CreatedByUserId);
            Assert.AreEqual(planRegistration.UpdatedByUserId, planRegistrationList[0].UpdatedByUserId);
            Assert.AreEqual(planRegistration.AssignedSiteId, planRegistrationList[0].AssignedSiteId);
            Assert.AreEqual(planRegistration.CommentOffice, planRegistrationList[0].CommentOffice);
            Assert.AreEqual(planRegistration.CommentOfficeAll, planRegistrationList[0].CommentOfficeAll);
            Assert.AreEqual(planRegistration.Date.ToString(), planRegistrationList[0].Date.ToString());
            Assert.AreEqual(planRegistration.Flex, planRegistrationList[0].Flex);
            Assert.AreEqual(planRegistration.MessageId, planRegistrationList[0].MessageId);
            Assert.AreEqual(planRegistration.NettoHours, planRegistrationList[0].NettoHours);
            Assert.AreEqual(planRegistration.PaiedOutFlex, planRegistrationList[0].PaiedOutFlex);
            Assert.AreEqual(planRegistration.Pause1Id, planRegistrationList[0].Pause1Id);
            Assert.AreEqual(planRegistration.Pause2Id, planRegistrationList[0].Pause2Id);
            Assert.AreEqual(planRegistration.Start1Id, planRegistrationList[0].Start1Id);
            Assert.AreEqual(planRegistration.Start2Id, planRegistrationList[0].Start2Id);
            Assert.AreEqual(planRegistration.Stop1Id, planRegistrationList[0].Stop1Id);
            Assert.AreEqual(planRegistration.Stop2Id, planRegistrationList[0].Stop2Id);
            Assert.AreEqual(planRegistration.PlanHours, planRegistrationList[0].PlanHours);
            Assert.AreEqual(planRegistration.PlanText, planRegistrationList[0].PlanText);
            Assert.AreEqual(planRegistration.SumFlex, planRegistrationList[0].SumFlex);
            Assert.AreEqual(planRegistration.Id, planRegistrationList[0].Id);
            Assert.AreEqual(1, planRegistrationList[0].Version);

            // versions
            Assert.AreEqual(Constants.WorkflowStates.Created, planRegistrationVersionsList[0].WorkflowState);
            Assert.AreEqual(planRegistration.CreatedByUserId, planRegistrationVersionsList[0].CreatedByUserId);
            Assert.AreEqual(planRegistration.UpdatedByUserId, planRegistrationVersionsList[0].UpdatedByUserId);
            Assert.AreEqual(planRegistration.AssignedSiteId, planRegistrationVersionsList[0].AssignedSiteId);
            Assert.AreEqual(planRegistration.CommentOffice, planRegistrationVersionsList[0].CommentOffice);
            Assert.AreEqual(planRegistration.CommentOfficeAll, planRegistrationVersionsList[0].CommentOfficeAll);
            Assert.AreEqual(planRegistration.Date.ToString(), planRegistrationVersionsList[0].Date.ToString());
            Assert.AreEqual(planRegistration.Flex, planRegistrationVersionsList[0].Flex);
            Assert.AreEqual(planRegistration.MessageId, planRegistrationVersionsList[0].MessageId);
            Assert.AreEqual(planRegistration.NettoHours, planRegistrationVersionsList[0].NettoHours);
            Assert.AreEqual(planRegistration.PaiedOutFlex, planRegistrationVersionsList[0].PaiedOutFlex);
            Assert.AreEqual(planRegistration.Pause1Id, planRegistrationVersionsList[0].Pause1Id);
            Assert.AreEqual(planRegistration.Pause2Id, planRegistrationVersionsList[0].Pause2Id);
            Assert.AreEqual(planRegistration.Start1Id, planRegistrationVersionsList[0].Start1Id);
            Assert.AreEqual(planRegistration.Start2Id, planRegistrationVersionsList[0].Start2Id);
            Assert.AreEqual(planRegistration.Stop1Id, planRegistrationVersionsList[0].Stop1Id);
            Assert.AreEqual(planRegistration.Stop2Id, planRegistrationVersionsList[0].Stop2Id);
            Assert.AreEqual(planRegistration.PlanHours, planRegistrationVersionsList[0].PlanHours);
            Assert.AreEqual(planRegistration.PlanText, planRegistrationVersionsList[0].PlanText);
            Assert.AreEqual(planRegistration.SumFlex, planRegistrationVersionsList[0].SumFlex);
            Assert.AreEqual(planRegistration.Id, planRegistrationVersionsList[0].PlanRegistrationId);
            Assert.AreEqual(1, planRegistrationVersionsList[0].Version);
        }

        [Test]
        public async Task ItemList_Update_DoesUpdate()
        {
            // Arrange
            var assignedSite = new AssignedSite
            {
                SiteId = 1,
            };
            await assignedSite.Create(DbContext);
            var assignedSiteTwo = new AssignedSite
            {
                SiteId = 2,
            };
            await assignedSiteTwo.Create(DbContext);
            var planRegistration = new PlanRegistration
            {
                AssignedSiteId = 1,
                CommentOffice = Guid.NewGuid().ToString(),
                CommentOfficeAll = Guid.NewGuid().ToString(),
                Flex = new Random().NextDouble(),
                Date = DateTime.Now,
                UpdatedByUserId = 1,
                CreatedByUserId = 1,
                MessageId = new Random().Next(1, 8),
                NettoHours = new Random().NextDouble(),
                PlanText = Guid.NewGuid().ToString(),
                PaiedOutFlex = new Random().NextDouble(),
                Pause1Id = new Random().Next(),
                Pause2Id = new Random().Next(),
                Start1Id = new Random().Next(),
                Start2Id = new Random().Next(),
                Stop1Id = new Random().Next(),
                Stop2Id = new Random().Next(),
                PlanHours = new Random().NextDouble(),
                SumFlex = new Random().NextDouble(),
            };
            await planRegistration.Create(DbContext);

            // Act
            
            var timePlanningOld = await DbContext.PlanRegistrations.AsNoTracking().FirstOrDefaultAsync();

            planRegistration.AssignedSiteId = 2;
            planRegistration.CommentOffice = Guid.NewGuid().ToString();
            planRegistration.CommentOfficeAll = Guid.NewGuid().ToString();
            planRegistration.Flex = new Random().NextDouble();
            planRegistration.Date = DateTime.Now;
            planRegistration.UpdatedByUserId = 1;
            planRegistration.MessageId = new Random().Next(1, 8);
            planRegistration.NettoHours = new Random().NextDouble();
            planRegistration.PlanText = Guid.NewGuid().ToString();
            planRegistration.Pause1Id = new Random().Next();
            planRegistration.Pause2Id = new Random().Next();
            planRegistration.Start1Id = new Random().Next();
            planRegistration.Start2Id = new Random().Next();
            planRegistration.Stop1Id = new Random().Next();
            planRegistration.Stop2Id = new Random().Next();
            planRegistration.PlanHours = new Random().NextDouble();
            planRegistration.SumFlex = new Random().NextDouble();
            await planRegistration.Update(DbContext);

            var planRegistrationList = DbContext.PlanRegistrations.AsNoTracking().ToList();
            var planRegistrationVersionsList = DbContext.PlanRegistrationVersions.AsNoTracking().ToList();

            // Assert
            Assert.AreEqual(1, planRegistrationList.Count);
            Assert.AreEqual(2, planRegistrationVersionsList.Count);
            Assert.AreEqual(Constants.WorkflowStates.Created, planRegistrationList[0].WorkflowState);
            Assert.AreEqual(planRegistration.CreatedByUserId, planRegistrationList[0].CreatedByUserId);
            Assert.AreEqual(planRegistration.UpdatedByUserId, planRegistrationList[0].UpdatedByUserId);
            Assert.AreEqual(planRegistration.AssignedSiteId, planRegistrationList[0].AssignedSiteId);
            Assert.AreEqual(planRegistration.CommentOffice, planRegistrationList[0].CommentOffice);
            Assert.AreEqual(planRegistration.CommentOfficeAll, planRegistrationList[0].CommentOfficeAll);
            Assert.AreEqual(planRegistration.Date.ToString(), planRegistrationList[0].Date.ToString());
            Assert.AreEqual(planRegistration.Flex, planRegistrationList[0].Flex);
            Assert.AreEqual(planRegistration.MessageId, planRegistrationList[0].MessageId);
            Assert.AreEqual(planRegistration.NettoHours, planRegistrationList[0].NettoHours);
            Assert.AreEqual(planRegistration.PaiedOutFlex, planRegistrationList[0].PaiedOutFlex);
            Assert.AreEqual(planRegistration.Pause1Id, planRegistrationList[0].Pause1Id);
            Assert.AreEqual(planRegistration.Pause2Id, planRegistrationList[0].Pause2Id);
            Assert.AreEqual(planRegistration.Start1Id, planRegistrationList[0].Start1Id);
            Assert.AreEqual(planRegistration.Start2Id, planRegistrationList[0].Start2Id);
            Assert.AreEqual(planRegistration.Stop1Id, planRegistrationList[0].Stop1Id);
            Assert.AreEqual(planRegistration.Stop2Id, planRegistrationList[0].Stop2Id);
            Assert.AreEqual(planRegistration.PlanHours, planRegistrationList[0].PlanHours);
            Assert.AreEqual(planRegistration.PlanText, planRegistrationList[0].PlanText);
            Assert.AreEqual(planRegistration.SumFlex, planRegistrationList[0].SumFlex);
            Assert.AreEqual(planRegistration.Id, planRegistrationList[0].Id);
            Assert.AreEqual(2, planRegistrationList[0].Version);

            // versions
            Assert.AreEqual(Constants.WorkflowStates.Created, planRegistrationVersionsList[0].WorkflowState);
            Assert.AreEqual(timePlanningOld.CreatedByUserId, planRegistrationVersionsList[0].CreatedByUserId);
            Assert.AreEqual(timePlanningOld.UpdatedByUserId, planRegistrationVersionsList[0].UpdatedByUserId);
            Assert.AreEqual(timePlanningOld.AssignedSiteId, planRegistrationVersionsList[0].AssignedSiteId);
            Assert.AreEqual(timePlanningOld.CommentOffice, planRegistrationVersionsList[0].CommentOffice);
            Assert.AreEqual(timePlanningOld.CommentOfficeAll, planRegistrationVersionsList[0].CommentOfficeAll);
            Assert.AreEqual(timePlanningOld.Date.ToString(), planRegistrationVersionsList[0].Date.ToString());
            Assert.AreEqual(timePlanningOld.Flex, planRegistrationVersionsList[0].Flex);
            Assert.AreEqual(timePlanningOld.MessageId, planRegistrationVersionsList[0].MessageId);
            Assert.AreEqual(timePlanningOld.NettoHours, planRegistrationVersionsList[0].NettoHours);
            Assert.AreEqual(timePlanningOld.PaiedOutFlex, planRegistrationVersionsList[0].PaiedOutFlex);
            Assert.AreEqual(timePlanningOld.Pause1Id, planRegistrationVersionsList[0].Pause1Id);
            Assert.AreEqual(timePlanningOld.Pause2Id, planRegistrationVersionsList[0].Pause2Id);
            Assert.AreEqual(timePlanningOld.Start1Id, planRegistrationVersionsList[0].Start1Id);
            Assert.AreEqual(timePlanningOld.Start2Id, planRegistrationVersionsList[0].Start2Id);
            Assert.AreEqual(timePlanningOld.Stop1Id, planRegistrationVersionsList[0].Stop1Id);
            Assert.AreEqual(timePlanningOld.Stop2Id, planRegistrationVersionsList[0].Stop2Id);
            Assert.AreEqual(timePlanningOld.PlanHours, planRegistrationVersionsList[0].PlanHours);
            Assert.AreEqual(timePlanningOld.PlanText, planRegistrationVersionsList[0].PlanText);
            Assert.AreEqual(timePlanningOld.SumFlex, planRegistrationVersionsList[0].SumFlex);
            Assert.AreEqual(timePlanningOld.Id, planRegistrationVersionsList[0].PlanRegistrationId);
            Assert.AreEqual(1, planRegistrationVersionsList[0].Version);

            Assert.AreEqual(Constants.WorkflowStates.Created, planRegistrationVersionsList[1].WorkflowState);
            Assert.AreEqual(planRegistration.CreatedByUserId, planRegistrationVersionsList[1].CreatedByUserId);
            Assert.AreEqual(planRegistration.UpdatedByUserId, planRegistrationVersionsList[1].UpdatedByUserId);
            Assert.AreEqual(planRegistration.AssignedSiteId, planRegistrationVersionsList[1].AssignedSiteId);
            Assert.AreEqual(planRegistration.CommentOffice, planRegistrationVersionsList[1].CommentOffice);
            Assert.AreEqual(planRegistration.CommentOfficeAll, planRegistrationVersionsList[1].CommentOfficeAll);
            Assert.AreEqual(planRegistration.Date.ToString(), planRegistrationVersionsList[1].Date.ToString());
            Assert.AreEqual(planRegistration.Flex, planRegistrationVersionsList[1].Flex);
            Assert.AreEqual(planRegistration.MessageId, planRegistrationVersionsList[1].MessageId);
            Assert.AreEqual(planRegistration.NettoHours, planRegistrationVersionsList[1].NettoHours);
            Assert.AreEqual(planRegistration.PaiedOutFlex, planRegistrationVersionsList[1].PaiedOutFlex);
            Assert.AreEqual(planRegistration.Pause1Id, planRegistrationVersionsList[1].Pause1Id);
            Assert.AreEqual(planRegistration.Pause2Id, planRegistrationVersionsList[1].Pause2Id);
            Assert.AreEqual(planRegistration.Start1Id, planRegistrationVersionsList[1].Start1Id);
            Assert.AreEqual(planRegistration.Start2Id, planRegistrationVersionsList[1].Start2Id);
            Assert.AreEqual(planRegistration.Stop1Id, planRegistrationVersionsList[1].Stop1Id);
            Assert.AreEqual(planRegistration.Stop2Id, planRegistrationVersionsList[1].Stop2Id);
            Assert.AreEqual(planRegistration.PlanHours, planRegistrationVersionsList[1].PlanHours);
            Assert.AreEqual(planRegistration.PlanText, planRegistrationVersionsList[1].PlanText);
            Assert.AreEqual(planRegistration.SumFlex, planRegistrationVersionsList[1].SumFlex);
            Assert.AreEqual(planRegistration.Id, planRegistrationVersionsList[1].PlanRegistrationId);
            Assert.AreEqual(2, planRegistrationVersionsList[1].Version);
        }

        [Test]
        public async Task ItemList_Delete_DoesDelete()
        {
            // Arrange
            var assignedSite = new AssignedSite
            {
                SiteId = 1,
            };
            await assignedSite.Create(DbContext);
            var assignedSiteTwo = new AssignedSite
            {
                SiteId = 2,
            };
            await assignedSiteTwo.Create(DbContext);
            var planRegistration = new PlanRegistration
            {
                AssignedSiteId = 1,
                CommentOffice = Guid.NewGuid().ToString(),
                CommentOfficeAll = Guid.NewGuid().ToString(),
                Flex = new Random().NextDouble(),
                Date = DateTime.Now,
                UpdatedByUserId = 1,
                CreatedByUserId = 1,
                MessageId = new Random().Next(1, 8),
                NettoHours = new Random().NextDouble(),
                PlanText = Guid.NewGuid().ToString(),
                PaiedOutFlex = new Random().NextDouble(),
                Pause1Id = new Random().Next(),
                Pause2Id = new Random().Next(),
                Start1Id = new Random().Next(),
                Start2Id = new Random().Next(),
                Stop1Id = new Random().Next(),
                Stop2Id = new Random().Next(),
                PlanHours = new Random().NextDouble(),
                SumFlex = new Random().NextDouble(),
            };
            await planRegistration.Create(DbContext);

            // Act

            var timePlanningOld = await DbContext.PlanRegistrations.AsNoTracking().FirstOrDefaultAsync();
            await planRegistration.Delete(DbContext);

            var planRegistrationList = DbContext.PlanRegistrations.AsNoTracking().ToList();
            var planRegistrationVersionsList = DbContext.PlanRegistrationVersions.AsNoTracking().ToList();

            // Assert
            Assert.AreEqual(1, planRegistrationList.Count);
            Assert.AreEqual(2, planRegistrationVersionsList.Count);
            Assert.AreEqual(Constants.WorkflowStates.Removed, planRegistrationList[0].WorkflowState);
            Assert.AreEqual(planRegistration.CreatedByUserId, planRegistrationList[0].CreatedByUserId);
            Assert.AreEqual(planRegistration.UpdatedByUserId, planRegistrationList[0].UpdatedByUserId);
            Assert.AreEqual(planRegistration.AssignedSiteId, planRegistrationList[0].AssignedSiteId);
            Assert.AreEqual(planRegistration.CommentOffice, planRegistrationList[0].CommentOffice);
            Assert.AreEqual(planRegistration.CommentOfficeAll, planRegistrationList[0].CommentOfficeAll);
            Assert.AreEqual(planRegistration.Date.ToString(), planRegistrationList[0].Date.ToString());
            Assert.AreEqual(planRegistration.Flex, planRegistrationList[0].Flex);
            Assert.AreEqual(planRegistration.MessageId, planRegistrationList[0].MessageId);
            Assert.AreEqual(planRegistration.NettoHours, planRegistrationList[0].NettoHours);
            Assert.AreEqual(planRegistration.PaiedOutFlex, planRegistrationList[0].PaiedOutFlex);
            Assert.AreEqual(planRegistration.Pause1Id, planRegistrationList[0].Pause1Id);
            Assert.AreEqual(planRegistration.Pause2Id, planRegistrationList[0].Pause2Id);
            Assert.AreEqual(planRegistration.Start1Id, planRegistrationList[0].Start1Id);
            Assert.AreEqual(planRegistration.Start2Id, planRegistrationList[0].Start2Id);
            Assert.AreEqual(planRegistration.Stop1Id, planRegistrationList[0].Stop1Id);
            Assert.AreEqual(planRegistration.Stop2Id, planRegistrationList[0].Stop2Id);
            Assert.AreEqual(planRegistration.PlanHours, planRegistrationList[0].PlanHours);
            Assert.AreEqual(planRegistration.PlanText, planRegistrationList[0].PlanText);
            Assert.AreEqual(planRegistration.SumFlex, planRegistrationList[0].SumFlex);
            Assert.AreEqual(planRegistration.Id, planRegistrationList[0].Id);
            Assert.AreEqual(2, planRegistrationList[0].Version);

            // versions
            Assert.AreEqual(Constants.WorkflowStates.Created, planRegistrationVersionsList[0].WorkflowState);
            Assert.AreEqual(timePlanningOld.CreatedByUserId, planRegistrationVersionsList[0].CreatedByUserId);
            Assert.AreEqual(timePlanningOld.UpdatedByUserId, planRegistrationVersionsList[0].UpdatedByUserId);
            Assert.AreEqual(timePlanningOld.AssignedSiteId, planRegistrationVersionsList[0].AssignedSiteId);
            Assert.AreEqual(timePlanningOld.CommentOffice, planRegistrationVersionsList[0].CommentOffice);
            Assert.AreEqual(timePlanningOld.CommentOfficeAll, planRegistrationVersionsList[0].CommentOfficeAll);
            Assert.AreEqual(timePlanningOld.Date.ToString(), planRegistrationVersionsList[0].Date.ToString());
            Assert.AreEqual(timePlanningOld.Flex, planRegistrationVersionsList[0].Flex);
            Assert.AreEqual(timePlanningOld.MessageId, planRegistrationVersionsList[0].MessageId);
            Assert.AreEqual(timePlanningOld.NettoHours, planRegistrationVersionsList[0].NettoHours);
            Assert.AreEqual(timePlanningOld.PaiedOutFlex, planRegistrationVersionsList[0].PaiedOutFlex);
            Assert.AreEqual(timePlanningOld.Pause1Id, planRegistrationVersionsList[0].Pause1Id);
            Assert.AreEqual(timePlanningOld.Pause2Id, planRegistrationVersionsList[0].Pause2Id);
            Assert.AreEqual(timePlanningOld.Start1Id, planRegistrationVersionsList[0].Start1Id);
            Assert.AreEqual(timePlanningOld.Start2Id, planRegistrationVersionsList[0].Start2Id);
            Assert.AreEqual(timePlanningOld.Stop1Id, planRegistrationVersionsList[0].Stop1Id);
            Assert.AreEqual(timePlanningOld.Stop2Id, planRegistrationVersionsList[0].Stop2Id);
            Assert.AreEqual(timePlanningOld.PlanHours, planRegistrationVersionsList[0].PlanHours);
            Assert.AreEqual(timePlanningOld.PlanText, planRegistrationVersionsList[0].PlanText);
            Assert.AreEqual(timePlanningOld.SumFlex, planRegistrationVersionsList[0].SumFlex);
            Assert.AreEqual(timePlanningOld.Id, planRegistrationVersionsList[0].PlanRegistrationId);
            Assert.AreEqual(1, planRegistrationVersionsList[0].Version);

            Assert.AreEqual(Constants.WorkflowStates.Removed, planRegistrationVersionsList[1].WorkflowState);
            Assert.AreEqual(planRegistration.CreatedByUserId, planRegistrationVersionsList[1].CreatedByUserId);
            Assert.AreEqual(planRegistration.UpdatedByUserId, planRegistrationVersionsList[1].UpdatedByUserId);
            Assert.AreEqual(planRegistration.AssignedSiteId, planRegistrationVersionsList[1].AssignedSiteId);
            Assert.AreEqual(planRegistration.CommentOffice, planRegistrationVersionsList[1].CommentOffice);
            Assert.AreEqual(planRegistration.CommentOfficeAll, planRegistrationVersionsList[1].CommentOfficeAll);
            Assert.AreEqual(planRegistration.Date.ToString(), planRegistrationVersionsList[1].Date.ToString());
            Assert.AreEqual(planRegistration.Flex, planRegistrationVersionsList[1].Flex);
            Assert.AreEqual(planRegistration.MessageId, planRegistrationVersionsList[1].MessageId);
            Assert.AreEqual(planRegistration.NettoHours, planRegistrationVersionsList[1].NettoHours);
            Assert.AreEqual(planRegistration.PaiedOutFlex, planRegistrationVersionsList[1].PaiedOutFlex);
            Assert.AreEqual(planRegistration.Pause1Id, planRegistrationVersionsList[1].Pause1Id);
            Assert.AreEqual(planRegistration.Pause2Id, planRegistrationVersionsList[1].Pause2Id);
            Assert.AreEqual(planRegistration.Start1Id, planRegistrationVersionsList[1].Start1Id);
            Assert.AreEqual(planRegistration.Start2Id, planRegistrationVersionsList[1].Start2Id);
            Assert.AreEqual(planRegistration.Stop1Id, planRegistrationVersionsList[1].Stop1Id);
            Assert.AreEqual(planRegistration.Stop2Id, planRegistrationVersionsList[1].Stop2Id);
            Assert.AreEqual(planRegistration.PlanHours, planRegistrationVersionsList[1].PlanHours);
            Assert.AreEqual(planRegistration.PlanText, planRegistrationVersionsList[1].PlanText);
            Assert.AreEqual(planRegistration.SumFlex, planRegistrationVersionsList[1].SumFlex);
            Assert.AreEqual(planRegistration.Id, planRegistrationVersionsList[1].PlanRegistrationId);
            Assert.AreEqual(2, planRegistrationVersionsList[1].Version);
        }
    }
}