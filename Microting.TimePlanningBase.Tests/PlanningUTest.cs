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
                SdkSitId = 1,
                CommentOffice = Guid.NewGuid().ToString(),
                CommentOfficeAll = Guid.NewGuid().ToString(),
                Flex = new Random().NextDouble(),
                Date = DateTime.Now,
                UpdatedByUserId = 1,
                CreatedByUserId = 1,
                MessageId = null,
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
                SumFlexStart = new Random().NextDouble(),
                IsSaturday = true,
                IsSunday = false,
                EffectiveNetHours = 8.5,
                NormalHours = 7.5,
                OvertimeHours = 1.0,
                WeekendHours = 0.0,
                NightHours = 0.5,
                HolidayHours = null,
                AbsenceHours = null,
                FirstWorkStartUtc = DateTime.UtcNow.AddHours(-8),
                LastWorkEndUtc = DateTime.UtcNow,
                RuleEngineCalculated = true,
                RuleEngineCalculatedAt = DateTime.UtcNow,
            };

            // Act
            await planRegistration.Create(DbContext);

            var planRegistrationList = DbContext.PlanRegistrations
                .AsNoTracking()
                .ToList();


            var planRegistrationVersionsList = DbContext.PlanRegistrationVersions.AsNoTracking().ToList();

            // Assert
            Assert.That(planRegistrationList.Count, Is.EqualTo(1));
            Assert.That(planRegistrationVersionsList.Count, Is.EqualTo(1));
            Assert.That(planRegistrationList[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));
            Assert.That(planRegistrationList[0].CreatedByUserId, Is.EqualTo(planRegistration.CreatedByUserId));
            Assert.That(planRegistrationList[0].UpdatedByUserId, Is.EqualTo(planRegistration.UpdatedByUserId));
            Assert.That(planRegistrationList[0].SdkSitId, Is.EqualTo(planRegistration.SdkSitId));
            Assert.That(planRegistrationList[0].CommentOffice, Is.EqualTo(planRegistration.CommentOffice));
            Assert.That(planRegistrationList[0].CommentOfficeAll, Is.EqualTo(planRegistration.CommentOfficeAll));
            Assert.That(planRegistration.Date.ToString(), Is.EqualTo(planRegistrationList[0].Date.ToString()));
            Assert.That(planRegistrationList[0].Flex, Is.EqualTo(planRegistration.Flex));
            Assert.That(planRegistrationList[0].MessageId, Is.EqualTo(planRegistration.MessageId));
            Assert.That(planRegistrationList[0].NettoHours, Is.EqualTo(planRegistration.NettoHours));
            Assert.That(planRegistrationList[0].PaiedOutFlex, Is.EqualTo(planRegistration.PaiedOutFlex));
            Assert.That(planRegistrationList[0].Pause1Id, Is.EqualTo(planRegistration.Pause1Id));
            Assert.That(planRegistrationList[0].Pause2Id, Is.EqualTo(planRegistration.Pause2Id));
            Assert.That(planRegistrationList[0].Start1Id, Is.EqualTo(planRegistration.Start1Id));
            Assert.That(planRegistrationList[0].Start2Id, Is.EqualTo(planRegistration.Start2Id));
            Assert.That(planRegistrationList[0].Stop1Id, Is.EqualTo(planRegistration.Stop1Id));
            Assert.That(planRegistrationList[0].Stop2Id, Is.EqualTo(planRegistration.Stop2Id));
            Assert.That(planRegistrationList[0].PlanHours, Is.EqualTo(planRegistration.PlanHours));
            Assert.That(planRegistrationList[0].PlanText, Is.EqualTo(planRegistration.PlanText));
            Assert.That(planRegistrationList[0].SumFlexStart, Is.EqualTo(planRegistration.SumFlexStart));
            Assert.That(planRegistrationList[0].Id, Is.EqualTo(planRegistration.Id));
            Assert.That(planRegistrationList[0].Version, Is.EqualTo(1));
            Assert.That(planRegistrationList[0].IsSaturday, Is.EqualTo(planRegistration.IsSaturday));
            Assert.That(planRegistrationList[0].IsSunday, Is.EqualTo(planRegistration.IsSunday));
            Assert.That(planRegistrationList[0].EffectiveNetHours, Is.EqualTo(planRegistration.EffectiveNetHours));
            Assert.That(planRegistrationList[0].NormalHours, Is.EqualTo(planRegistration.NormalHours));
            Assert.That(planRegistrationList[0].OvertimeHours, Is.EqualTo(planRegistration.OvertimeHours));
            Assert.That(planRegistrationList[0].WeekendHours, Is.EqualTo(planRegistration.WeekendHours));
            Assert.That(planRegistrationList[0].NightHours, Is.EqualTo(planRegistration.NightHours));
            Assert.That(planRegistrationList[0].HolidayHours, Is.EqualTo(planRegistration.HolidayHours));
            Assert.That(planRegistrationList[0].AbsenceHours, Is.EqualTo(planRegistration.AbsenceHours));
            Assert.That(planRegistrationList[0].RuleEngineCalculated, Is.EqualTo(planRegistration.RuleEngineCalculated));
            Assert.That(planRegistrationList[0].FirstWorkStartUtc?.ToString(), Is.EqualTo(planRegistration.FirstWorkStartUtc?.ToString()));
            Assert.That(planRegistrationList[0].LastWorkEndUtc?.ToString(), Is.EqualTo(planRegistration.LastWorkEndUtc?.ToString()));
            Assert.That(planRegistrationList[0].RuleEngineCalculatedAt?.ToString(), Is.EqualTo(planRegistration.RuleEngineCalculatedAt?.ToString()));

            // versions
            Assert.That(planRegistrationVersionsList[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));
            Assert.That(planRegistrationVersionsList[0].CreatedByUserId, Is.EqualTo(planRegistration.CreatedByUserId));
            Assert.That(planRegistrationVersionsList[0].UpdatedByUserId, Is.EqualTo(planRegistration.UpdatedByUserId));
            Assert.That(planRegistrationVersionsList[0].SdkSitId, Is.EqualTo(planRegistration.SdkSitId));
            Assert.That(planRegistrationVersionsList[0].CommentOffice, Is.EqualTo(planRegistration.CommentOffice));
            Assert.That(planRegistrationVersionsList[0].CommentOfficeAll, Is.EqualTo(planRegistration.CommentOfficeAll));
            Assert.That(planRegistration.Date.ToString(), Is.EqualTo(planRegistrationVersionsList[0].Date.ToString()));
            Assert.That(planRegistrationVersionsList[0].Flex, Is.EqualTo(planRegistration.Flex));
            Assert.That(planRegistrationVersionsList[0].MessageId, Is.EqualTo(planRegistration.MessageId));
            Assert.That(planRegistrationVersionsList[0].NettoHours, Is.EqualTo(planRegistration.NettoHours));
            Assert.That(planRegistrationVersionsList[0].PaiedOutFlex, Is.EqualTo(planRegistration.PaiedOutFlex));
            Assert.That(planRegistrationVersionsList[0].Pause1Id, Is.EqualTo(planRegistration.Pause1Id));
            Assert.That(planRegistrationVersionsList[0].Pause2Id, Is.EqualTo(planRegistration.Pause2Id));
            Assert.That(planRegistrationVersionsList[0].Start1Id, Is.EqualTo(planRegistration.Start1Id));
            Assert.That(planRegistrationVersionsList[0].Start2Id, Is.EqualTo(planRegistration.Start2Id));
            Assert.That(planRegistrationVersionsList[0].Stop1Id, Is.EqualTo(planRegistration.Stop1Id));
            Assert.That(planRegistrationVersionsList[0].Stop2Id, Is.EqualTo(planRegistration.Stop2Id));
            Assert.That(planRegistrationVersionsList[0].PlanHours, Is.EqualTo(planRegistration.PlanHours));
            Assert.That(planRegistrationVersionsList[0].PlanText, Is.EqualTo(planRegistration.PlanText));
            Assert.That(planRegistrationVersionsList[0].SumFlexStart, Is.EqualTo(planRegistration.SumFlexStart));
            Assert.That(planRegistrationVersionsList[0].PlanRegistrationId, Is.EqualTo(planRegistration.Id));
            Assert.That(planRegistrationVersionsList[0].Version, Is.EqualTo(1));
            Assert.That(planRegistrationVersionsList[0].IsSaturday, Is.EqualTo(planRegistration.IsSaturday));
            Assert.That(planRegistrationVersionsList[0].IsSunday, Is.EqualTo(planRegistration.IsSunday));
            Assert.That(planRegistrationVersionsList[0].EffectiveNetHours, Is.EqualTo(planRegistration.EffectiveNetHours));
            Assert.That(planRegistrationVersionsList[0].NormalHours, Is.EqualTo(planRegistration.NormalHours));
            Assert.That(planRegistrationVersionsList[0].OvertimeHours, Is.EqualTo(planRegistration.OvertimeHours));
            Assert.That(planRegistrationVersionsList[0].WeekendHours, Is.EqualTo(planRegistration.WeekendHours));
            Assert.That(planRegistrationVersionsList[0].NightHours, Is.EqualTo(planRegistration.NightHours));
            Assert.That(planRegistrationVersionsList[0].HolidayHours, Is.EqualTo(planRegistration.HolidayHours));
            Assert.That(planRegistrationVersionsList[0].AbsenceHours, Is.EqualTo(planRegistration.AbsenceHours));
            Assert.That(planRegistrationVersionsList[0].RuleEngineCalculated, Is.EqualTo(planRegistration.RuleEngineCalculated));
            Assert.That(planRegistrationVersionsList[0].FirstWorkStartUtc?.ToString(), Is.EqualTo(planRegistration.FirstWorkStartUtc?.ToString()));
            Assert.That(planRegistrationVersionsList[0].LastWorkEndUtc?.ToString(), Is.EqualTo(planRegistration.LastWorkEndUtc?.ToString()));
            Assert.That(planRegistrationVersionsList[0].RuleEngineCalculatedAt?.ToString(), Is.EqualTo(planRegistration.RuleEngineCalculatedAt?.ToString()));
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
                SdkSitId = 1,
                CommentOffice = Guid.NewGuid().ToString(),
                CommentOfficeAll = Guid.NewGuid().ToString(),
                Flex = new Random().NextDouble(),
                Date = DateTime.Now,
                UpdatedByUserId = 1,
                CreatedByUserId = 1,
                MessageId = null,
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
                SumFlexStart = new Random().NextDouble(),
                IsSaturday = false,
                IsSunday = true,
                EffectiveNetHours = 7.0,
                NormalHours = 7.0,
                OvertimeHours = null,
                WeekendHours = 0.0,
            };
            await planRegistration.Create(DbContext);

            // Act

            var timePlanningOld = await DbContext.PlanRegistrations.AsNoTracking().FirstOrDefaultAsync();

            planRegistration.SdkSitId = 2;
            planRegistration.CommentOffice = Guid.NewGuid().ToString();
            planRegistration.CommentOfficeAll = Guid.NewGuid().ToString();
            planRegistration.Flex = new Random().NextDouble();
            planRegistration.Date = DateTime.Now;
            planRegistration.UpdatedByUserId = 1;
            planRegistration.MessageId = null;
            planRegistration.NettoHours = new Random().NextDouble();
            planRegistration.PlanText = Guid.NewGuid().ToString();
            planRegistration.Pause1Id = new Random().Next();
            planRegistration.Pause2Id = new Random().Next();
            planRegistration.Start1Id = new Random().Next();
            planRegistration.Start2Id = new Random().Next();
            planRegistration.Stop1Id = new Random().Next();
            planRegistration.Stop2Id = new Random().Next();
            planRegistration.PlanHours = new Random().NextDouble();
            planRegistration.SumFlexStart = new Random().NextDouble();
            planRegistration.IsSaturday = true;
            planRegistration.IsSunday = false;
            planRegistration.EffectiveNetHours = 9.5;
            planRegistration.NormalHours = 8.0;
            planRegistration.OvertimeHours = 1.5;
            await planRegistration.Update(DbContext);

            var planRegistrationList = DbContext.PlanRegistrations.AsNoTracking().ToList();
            var planRegistrationVersionsList = DbContext.PlanRegistrationVersions.AsNoTracking().ToList();

            // Assert
            Assert.That(planRegistrationList.Count, Is.EqualTo(1));
            Assert.That(planRegistrationVersionsList.Count, Is.EqualTo(2));
            Assert.That(planRegistrationList[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));
            Assert.That(planRegistrationList[0].CreatedByUserId, Is.EqualTo(planRegistration.CreatedByUserId));
            Assert.That(planRegistrationList[0].UpdatedByUserId, Is.EqualTo(planRegistration.UpdatedByUserId));
            Assert.That(planRegistrationList[0].SdkSitId, Is.EqualTo(planRegistration.SdkSitId));
            Assert.That(planRegistrationList[0].CommentOffice, Is.EqualTo(planRegistration.CommentOffice));
            Assert.That(planRegistrationList[0].CommentOfficeAll, Is.EqualTo(planRegistration.CommentOfficeAll));
            Assert.That(planRegistration.Date.ToString(), Is.EqualTo(planRegistrationList[0].Date.ToString()));
            Assert.That(planRegistrationList[0].Flex, Is.EqualTo(planRegistration.Flex));
            Assert.That(planRegistrationList[0].MessageId, Is.EqualTo(planRegistration.MessageId));
            Assert.That(planRegistrationList[0].NettoHours, Is.EqualTo(planRegistration.NettoHours));
            Assert.That(planRegistrationList[0].PaiedOutFlex, Is.EqualTo(planRegistration.PaiedOutFlex));
            Assert.That(planRegistrationList[0].Pause1Id, Is.EqualTo(planRegistration.Pause1Id));
            Assert.That(planRegistrationList[0].Pause2Id, Is.EqualTo(planRegistration.Pause2Id));
            Assert.That(planRegistrationList[0].Start1Id, Is.EqualTo(planRegistration.Start1Id));
            Assert.That(planRegistrationList[0].Start2Id, Is.EqualTo(planRegistration.Start2Id));
            Assert.That(planRegistrationList[0].Stop1Id, Is.EqualTo(planRegistration.Stop1Id));
            Assert.That(planRegistrationList[0].Stop2Id, Is.EqualTo(planRegistration.Stop2Id));
            Assert.That(planRegistrationList[0].PlanHours, Is.EqualTo(planRegistration.PlanHours));
            Assert.That(planRegistrationList[0].PlanText, Is.EqualTo(planRegistration.PlanText));
            Assert.That(planRegistrationList[0].SumFlexStart, Is.EqualTo(planRegistration.SumFlexStart));
            Assert.That(planRegistrationList[0].Id, Is.EqualTo(planRegistration.Id));
            Assert.That(planRegistrationList[0].Version, Is.EqualTo(2));

            // versions
            Assert.That(planRegistrationVersionsList[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));
            Assert.That(planRegistrationVersionsList[0].CreatedByUserId, Is.EqualTo(timePlanningOld.CreatedByUserId));
            Assert.That(planRegistrationVersionsList[0].UpdatedByUserId, Is.EqualTo(timePlanningOld.UpdatedByUserId));
            Assert.That(planRegistrationVersionsList[0].SdkSitId, Is.EqualTo(timePlanningOld.SdkSitId));
            Assert.That(planRegistrationVersionsList[0].CommentOffice, Is.EqualTo(timePlanningOld.CommentOffice));
            Assert.That(planRegistrationVersionsList[0].CommentOfficeAll, Is.EqualTo(timePlanningOld.CommentOfficeAll));
            Assert.That(timePlanningOld.Date.ToString(), Is.EqualTo(planRegistrationVersionsList[0].Date.ToString()));
            Assert.That(planRegistrationVersionsList[0].Flex, Is.EqualTo(timePlanningOld.Flex));
            Assert.That(planRegistrationVersionsList[0].MessageId, Is.EqualTo(timePlanningOld.MessageId));
            Assert.That(planRegistrationVersionsList[0].NettoHours, Is.EqualTo(timePlanningOld.NettoHours));
            Assert.That(planRegistrationVersionsList[0].PaiedOutFlex, Is.EqualTo(timePlanningOld.PaiedOutFlex));
            Assert.That(planRegistrationVersionsList[0].Pause1Id, Is.EqualTo(timePlanningOld.Pause1Id));
            Assert.That(planRegistrationVersionsList[0].Pause2Id, Is.EqualTo(timePlanningOld.Pause2Id));
            Assert.That(planRegistrationVersionsList[0].Start1Id, Is.EqualTo(timePlanningOld.Start1Id));
            Assert.That(planRegistrationVersionsList[0].Start2Id, Is.EqualTo(timePlanningOld.Start2Id));
            Assert.That(planRegistrationVersionsList[0].Stop1Id, Is.EqualTo(timePlanningOld.Stop1Id));
            Assert.That(planRegistrationVersionsList[0].Stop2Id, Is.EqualTo(timePlanningOld.Stop2Id));
            Assert.That(planRegistrationVersionsList[0].PlanHours, Is.EqualTo(timePlanningOld.PlanHours));
            Assert.That(planRegistrationVersionsList[0].PlanText, Is.EqualTo(timePlanningOld.PlanText));
            Assert.That(planRegistrationVersionsList[0].SumFlexStart, Is.EqualTo(timePlanningOld.SumFlexStart));
            Assert.That(planRegistrationVersionsList[0].PlanRegistrationId, Is.EqualTo(timePlanningOld.Id));
            Assert.That(planRegistrationVersionsList[0].Version, Is.EqualTo(1));

            Assert.That(planRegistrationVersionsList[1].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));
            Assert.That(planRegistrationVersionsList[1].CreatedByUserId, Is.EqualTo(planRegistration.CreatedByUserId));
            Assert.That(planRegistrationVersionsList[1].UpdatedByUserId, Is.EqualTo(planRegistration.UpdatedByUserId));
            Assert.That(planRegistrationVersionsList[1].SdkSitId, Is.EqualTo(planRegistration.SdkSitId));
            Assert.That(planRegistrationVersionsList[1].CommentOffice, Is.EqualTo(planRegistration.CommentOffice));
            Assert.That(planRegistrationVersionsList[1].CommentOfficeAll, Is.EqualTo(planRegistration.CommentOfficeAll));
            Assert.That(planRegistration.Date.ToString(), Is.EqualTo(planRegistrationVersionsList[1].Date.ToString()));
            Assert.That(planRegistrationVersionsList[1].Flex, Is.EqualTo(planRegistration.Flex));
            Assert.That(planRegistrationVersionsList[1].MessageId, Is.EqualTo(planRegistration.MessageId));
            Assert.That(planRegistrationVersionsList[1].NettoHours, Is.EqualTo(planRegistration.NettoHours));
            Assert.That(planRegistrationVersionsList[1].PaiedOutFlex, Is.EqualTo(planRegistration.PaiedOutFlex));
            Assert.That(planRegistrationVersionsList[1].Pause1Id, Is.EqualTo(planRegistration.Pause1Id));
            Assert.That(planRegistrationVersionsList[1].Pause2Id, Is.EqualTo(planRegistration.Pause2Id));
            Assert.That(planRegistrationVersionsList[1].Start1Id, Is.EqualTo(planRegistration.Start1Id));
            Assert.That(planRegistrationVersionsList[1].Start2Id, Is.EqualTo(planRegistration.Start2Id));
            Assert.That(planRegistrationVersionsList[1].Stop1Id, Is.EqualTo(planRegistration.Stop1Id));
            Assert.That(planRegistrationVersionsList[1].Stop2Id, Is.EqualTo(planRegistration.Stop2Id));
            Assert.That(planRegistrationVersionsList[1].PlanHours, Is.EqualTo(planRegistration.PlanHours));
            Assert.That(planRegistrationVersionsList[1].PlanText, Is.EqualTo(planRegistration.PlanText));
            Assert.That(planRegistrationVersionsList[1].SumFlexStart, Is.EqualTo(planRegistration.SumFlexStart));
            Assert.That(planRegistrationVersionsList[1].PlanRegistrationId, Is.EqualTo(planRegistration.Id));
            Assert.That(planRegistrationVersionsList[1].Version, Is.EqualTo(2));
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
                SdkSitId = 1,
                CommentOffice = Guid.NewGuid().ToString(),
                CommentOfficeAll = Guid.NewGuid().ToString(),
                Flex = new Random().NextDouble(),
                Date = DateTime.Now,
                UpdatedByUserId = 1,
                CreatedByUserId = 1,
                MessageId = null,
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
                SumFlexStart = new Random().NextDouble(),
            };
            await planRegistration.Create(DbContext);

            // Act

            var timePlanningOld = await DbContext.PlanRegistrations.AsNoTracking().FirstOrDefaultAsync();
            await planRegistration.Delete(DbContext);

            var planRegistrationList = DbContext.PlanRegistrations.AsNoTracking().ToList();
            var planRegistrationVersionsList = DbContext.PlanRegistrationVersions.AsNoTracking().ToList();

            // Assert
            Assert.That(planRegistrationList.Count, Is.EqualTo(1));
            Assert.That(planRegistrationVersionsList.Count, Is.EqualTo(2));
            Assert.That(planRegistrationList[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Removed));
            Assert.That(planRegistrationList[0].CreatedByUserId, Is.EqualTo(planRegistration.CreatedByUserId));
            Assert.That(planRegistrationList[0].UpdatedByUserId, Is.EqualTo(planRegistration.UpdatedByUserId));
            Assert.That(planRegistrationList[0].SdkSitId, Is.EqualTo(planRegistration.SdkSitId));
            Assert.That(planRegistrationList[0].CommentOffice, Is.EqualTo(planRegistration.CommentOffice));
            Assert.That(planRegistrationList[0].CommentOfficeAll, Is.EqualTo(planRegistration.CommentOfficeAll));
            Assert.That(planRegistration.Date.ToString(), Is.EqualTo(planRegistrationList[0].Date.ToString()));
            Assert.That(planRegistrationList[0].Flex, Is.EqualTo(planRegistration.Flex));
            Assert.That(planRegistrationList[0].MessageId, Is.EqualTo(planRegistration.MessageId));
            Assert.That(planRegistrationList[0].NettoHours, Is.EqualTo(planRegistration.NettoHours));
            Assert.That(planRegistrationList[0].PaiedOutFlex, Is.EqualTo(planRegistration.PaiedOutFlex));
            Assert.That(planRegistrationList[0].Pause1Id, Is.EqualTo(planRegistration.Pause1Id));
            Assert.That(planRegistrationList[0].Pause2Id, Is.EqualTo(planRegistration.Pause2Id));
            Assert.That(planRegistrationList[0].Start1Id, Is.EqualTo(planRegistration.Start1Id));
            Assert.That(planRegistrationList[0].Start2Id, Is.EqualTo(planRegistration.Start2Id));
            Assert.That(planRegistrationList[0].Stop1Id, Is.EqualTo(planRegistration.Stop1Id));
            Assert.That(planRegistrationList[0].Stop2Id, Is.EqualTo(planRegistration.Stop2Id));
            Assert.That(planRegistrationList[0].PlanHours, Is.EqualTo(planRegistration.PlanHours));
            Assert.That(planRegistrationList[0].PlanText, Is.EqualTo(planRegistration.PlanText));
            Assert.That(planRegistrationList[0].SumFlexStart, Is.EqualTo(planRegistration.SumFlexStart));
            Assert.That(planRegistrationList[0].Id, Is.EqualTo(planRegistration.Id));
            Assert.That(planRegistrationList[0].Version, Is.EqualTo(2));

            // versions
            Assert.That(planRegistrationVersionsList[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));
            Assert.That(planRegistrationVersionsList[0].CreatedByUserId, Is.EqualTo(timePlanningOld.CreatedByUserId));
            Assert.That(planRegistrationVersionsList[0].UpdatedByUserId, Is.EqualTo(timePlanningOld.UpdatedByUserId));
            Assert.That(planRegistrationVersionsList[0].SdkSitId, Is.EqualTo(timePlanningOld.SdkSitId));
            Assert.That(planRegistrationVersionsList[0].CommentOffice, Is.EqualTo(timePlanningOld.CommentOffice));
            Assert.That(planRegistrationVersionsList[0].CommentOfficeAll, Is.EqualTo(timePlanningOld.CommentOfficeAll));
            Assert.That(timePlanningOld.Date.ToString(), Is.EqualTo(planRegistrationVersionsList[0].Date.ToString()));
            Assert.That(planRegistrationVersionsList[0].Flex, Is.EqualTo(timePlanningOld.Flex));
            Assert.That(planRegistrationVersionsList[0].MessageId, Is.EqualTo(timePlanningOld.MessageId));
            Assert.That(planRegistrationVersionsList[0].NettoHours, Is.EqualTo(timePlanningOld.NettoHours));
            Assert.That(planRegistrationVersionsList[0].PaiedOutFlex, Is.EqualTo(timePlanningOld.PaiedOutFlex));
            Assert.That(planRegistrationVersionsList[0].Pause1Id, Is.EqualTo(timePlanningOld.Pause1Id));
            Assert.That(planRegistrationVersionsList[0].Pause2Id, Is.EqualTo(timePlanningOld.Pause2Id));
            Assert.That(planRegistrationVersionsList[0].Start1Id, Is.EqualTo(timePlanningOld.Start1Id));
            Assert.That(planRegistrationVersionsList[0].Start2Id, Is.EqualTo(timePlanningOld.Start2Id));
            Assert.That(planRegistrationVersionsList[0].Stop1Id, Is.EqualTo(timePlanningOld.Stop1Id));
            Assert.That(planRegistrationVersionsList[0].Stop2Id, Is.EqualTo(timePlanningOld.Stop2Id));
            Assert.That(planRegistrationVersionsList[0].PlanHours, Is.EqualTo(timePlanningOld.PlanHours));
            Assert.That(planRegistrationVersionsList[0].PlanText, Is.EqualTo(timePlanningOld.PlanText));
            Assert.That(planRegistrationVersionsList[0].SumFlexStart, Is.EqualTo(timePlanningOld.SumFlexStart));
            Assert.That(planRegistrationVersionsList[0].PlanRegistrationId, Is.EqualTo(timePlanningOld.Id));
            Assert.That(planRegistrationVersionsList[0].Version, Is.EqualTo(1));

            Assert.That(planRegistrationVersionsList[1].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Removed));
            Assert.That(planRegistrationVersionsList[1].CreatedByUserId, Is.EqualTo(planRegistration.CreatedByUserId));
            Assert.That(planRegistrationVersionsList[1].UpdatedByUserId, Is.EqualTo(planRegistration.UpdatedByUserId));
            Assert.That(planRegistrationVersionsList[1].SdkSitId, Is.EqualTo(planRegistration.SdkSitId));
            Assert.That(planRegistrationVersionsList[1].CommentOffice, Is.EqualTo(planRegistration.CommentOffice));
            Assert.That(planRegistrationVersionsList[1].CommentOfficeAll, Is.EqualTo(planRegistration.CommentOfficeAll));
            Assert.That(planRegistration.Date.ToString(), Is.EqualTo(planRegistrationVersionsList[1].Date.ToString()));
            Assert.That(planRegistrationVersionsList[1].Flex, Is.EqualTo(planRegistration.Flex));
            Assert.That(planRegistrationVersionsList[1].MessageId, Is.EqualTo(planRegistration.MessageId));
            Assert.That(planRegistrationVersionsList[1].NettoHours, Is.EqualTo(planRegistration.NettoHours));
            Assert.That(planRegistrationVersionsList[1].PaiedOutFlex, Is.EqualTo(planRegistration.PaiedOutFlex));
            Assert.That(planRegistrationVersionsList[1].Pause1Id, Is.EqualTo(planRegistration.Pause1Id));
            Assert.That(planRegistrationVersionsList[1].Pause2Id, Is.EqualTo(planRegistration.Pause2Id));
            Assert.That(planRegistrationVersionsList[1].Start1Id, Is.EqualTo(planRegistration.Start1Id));
            Assert.That(planRegistrationVersionsList[1].Start2Id, Is.EqualTo(planRegistration.Start2Id));
            Assert.That(planRegistrationVersionsList[1].Stop1Id, Is.EqualTo(planRegistration.Stop1Id));
            Assert.That(planRegistrationVersionsList[1].Stop2Id, Is.EqualTo(planRegistration.Stop2Id));
            Assert.That(planRegistrationVersionsList[1].PlanHours, Is.EqualTo(planRegistration.PlanHours));
            Assert.That(planRegistrationVersionsList[1].PlanText, Is.EqualTo(planRegistration.PlanText));
            Assert.That(planRegistrationVersionsList[1].SumFlexStart, Is.EqualTo(planRegistration.SumFlexStart));
            Assert.That(planRegistrationVersionsList[1].PlanRegistrationId, Is.EqualTo(planRegistration.Id));
            Assert.That(planRegistrationVersionsList[1].Version, Is.EqualTo(2));
        }
    }
}