/*
The MIT License (MIT)

Copyright (c) 2007 - 2021 Microting A/S

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

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microting.eForm.Dto;
using Microting.eForm.Infrastructure.Constants;
using Microting.eForm.Infrastructure.Data.Entities;
using Microting.eForm.Infrastructure.Models;

namespace Microting.TimePlanningBase.Infrastructure.Data.Entities
{
    using System;

    public class PlanRegistration : PnBase
    {
        public int SdkSitId { get; set; }

        public DateTime Date { get; set; }

        public string PlanText { get; set; }

        public double PlanHours { get; set; }

        public int Start1Id{ get; set; }

        public int Stop1Id { get; set; }

        public int Pause1Id { get; set; }

        public int Start2Id { get; set; }

        public int Stop2Id { get; set; }

        public int Pause2Id { get; set; }

        public double NettoHours { get; set; }

        public double Flex { get; set; }

        public double SumFlex { get; set; }

        public double PaiedOutFlex { get; set; }

        public int? MessageId { get; set; }

        public virtual Message Message { get; set; }

        public string CommentOffice { get; set; }

        public string CommentOfficeAll { get; set; }

        public int StatusCaseId { get; set; }

        public string WorkerComment { get; set; }

        public readonly List<string> Options = new();

        public PlanRegistration()
        {
            int minute = 0;
            int hour = 0;
            for (int i = 0; i < 288; i++)
            {
                Options.Add($"{hour:00}:{minute:00}");
                minute += 5;
                if (minute == 60)
                {
                    minute = 0;
                    hour++;
                }
            }
        }

        public async Task<int> DeployResults(int maxHistoryDays, int eFormId, eFormCore.Core core, Site siteInfo, int folderId, string messageText)
        {
            if (StatusCaseId != 0)
            {
                    await core.CaseDelete(StatusCaseId);
            }
            await using var sdkDbContext = core.DbContextHelper.GetDbContext();
            var language = await sdkDbContext.Languages.SingleAsync(x => x.Id == siteInfo.LanguageId);
            var folder = await sdkDbContext.Folders.SingleOrDefaultAsync(x => x.Id == folderId);
            var mainElement = await core.ReadeForm(eFormId, language);
            CultureInfo ci = new CultureInfo(language.LanguageCode);
            mainElement.Label = Date.ToString("dddd dd. MMM yyyy", ci);
            mainElement.EndDate = DateTime.UtcNow.AddDays(maxHistoryDays);
            DateTime startDate = new DateTime(2020, 1, 1);
            mainElement.DisplayOrder = (startDate - Date).Days;
            DataElement element = (DataElement)mainElement.ElementList.First();
            element.Label = mainElement.Label;
            element.DoneButtonEnabled = false;
            CDataValue cDataValue = new CDataValue
            {
                InderValue = $"<strong>NettoHours: {NettoHours:0.00}</strong><br/>" +
                             $"{messageText}"
            };
            element.Description = cDataValue;
            DataItem dataItem = element.DataItemList.First();
            dataItem.Color = Constants.FieldColors.Yellow;
            dataItem.Label = $"<strong>Date: {Date.ToString("dddd dd. MMM yyyy", ci)}</strong>";
            cDataValue = new CDataValue
            {
                InderValue = $"PlanText: {PlanText}<br/>"+
                             $"PlanHours: {PlanHours}<br/><br/>" +
                             $"Shift 1 start: {Options[Start1Id > 0 ? Start1Id - 1 : 0]}<br/>" +
                             $"Shift 1 pause: {Options[Pause1Id > 0 ? Pause1Id - 1 : 0]}<br/>" +
                             $"Shift 1 end: {Options[Stop1Id > 0 ? Stop1Id - 1 : 0]}<br/><br/>" +
                             $"Shift 2 start: {Options[Start2Id > 0 ? Start2Id - 1 : 0]}<br/>" +
                             $"Shift 2 pause: {Options[Pause2Id > 0 ? Pause2Id - 1 : 0]}<br/>" +
                             $"Shift 2 end: {Options[Stop2Id > 0 ? Stop2Id - 1 : 0]}<br/><br/>" +
                             $"<strong>NettoHours: {NettoHours:0.00}</strong><br/><br/>" +
                             $"Flex: {Flex:0.00)}<br/>" +
                             $"SumFlex: {SumFlex:0.00}<br/>" +
                             $"PaidOutFlex: {PaiedOutFlex:0.00}<br/><br/>" +
                             $"Message: {messageText}<br/><br/>"+
                             "<strong>Comments:</strong><br/>" +
                             $"{WorkerComment}<br/><br/>" +
                             "<strong>Comment office:</strong><br/>" +
                             $"{CommentOffice}<br/><br/>" +
                             "<strong>Comment office all:</strong><br/>" +
                             $"{CommentOffice}<br/>"
            };
            dataItem.Description = cDataValue;

            if (folder != null) mainElement.CheckListFolderName = folder.MicrotingUid.ToString();

            return (int)await core.CaseCreate(mainElement, "", (int)siteInfo.MicrotingUid, folderId);
        }
    }
}