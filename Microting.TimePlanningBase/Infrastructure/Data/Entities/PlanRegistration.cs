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

        public double SumFlexStart { get; set; }

        public double SumFlexEnd { get; set; }

        public double PaiedOutFlex { get; set; }

        public int? MessageId { get; set; }

        public virtual Message Message { get; set; }

        public string CommentOffice { get; set; }

        public string CommentOfficeAll { get; set; }

        public int StatusCaseId { get; set; }

        public string WorkerComment { get; set; }
        
        public bool DataFromDevice { get; set; }

        public readonly List<string> Options = new();

        public int? RegistrationDeviceId { get; set; }

        public DateTime? Start1StartedAt { get; set; }

        public DateTime? Stop1StoppedAt { get; set; }

        public DateTime? Pause1StartedAt { get; set; }

        public DateTime? Pause1StoppedAt { get; set; }

        public DateTime? Start2StartedAt { get; set; }

        public DateTime? Stop2StoppedAt { get; set; }

        public DateTime? Pause2StartedAt { get; set; }

        public DateTime? Pause2StoppedAt { get; set; }

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
    }
}