﻿/*
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

namespace Microting.TimePlanningBase.Infrastructure.Data.Entities
{
    public class AssignedSiteVersion: PnBase
    {
        public int SiteId { get; set; }

        public int AssignedSiteId { get; set; }

        public int? CaseMicrotingUid { get; set; }

        // minutes from midnight for the start of the shift (0-1440) monday
        public int StartMonday { get; set; }
        // minutes from midnight for the end of the shift (0-1440) monday
        public int EndMonday { get; set; }
        // minutes of break for the shift (0-1440) monday
        public int BreakMonday { get; set; }
        // minutes from midnight for the start of the shift (0-1440) tuesday
        public int StartTuesday { get; set; }
        // minutes from midnight for the end of the shift (0-1440) tuesday
        public int EndTuesday { get; set; }
        // minutes of break for the shift (0-1440) tuesday
        public int BreakTuesday { get; set; }
        // minutes from midnight for the start of the shift (0-1440) wednesday
        public int StartWednesday { get; set; }
        // minutes from midnight for the end of the shift (0-1440) wednesday
        public int EndWednesday { get; set; }
        // minutes of break for the shift (0-1440) wednesday
        public int BreakWednesday { get; set; }
        // minutes from midnight for the start of the shift (0-1440) thursday
        public int StartThursday { get; set; }
        // minutes from midnight for the end of the shift (0-1440) thursday
        public int EndThursday { get; set; }
        // minutes of break for the shift (0-1440) thursday
        public int BreakThursday { get; set; }
        // minutes from midnight for the start of the shift (0-1440) friday
        public int StartFriday { get; set; }
        // minutes from midnight for the end of the shift (0-1440) friday
        public int EndFriday { get; set; }
        // minutes of break for the shift (0-1440) friday
        public int BreakFriday { get; set; }
        // minutes from midnight for the start of the shift (0-1440) saturday
        public int StartSaturday { get; set; }
        // minutes from midnight for the end of the shift (0-1440) saturday
        public int EndSaturday { get; set; }
        // minutes of break for the shift (0-1440) saturday
        public int BreakSaturday { get; set; }
        // minutes from midnight for the start of the shift (0-1440) sunday
        public int StartSunday { get; set; }
        // minutes from midnight for the end of the shift (0-1440) sunday
        public int EndSunday { get; set; }
        // minutes of break for the shift (0-1440) sunday
        public int BreakSunday { get; set; }

        public bool Resigned { get; set; }
    }
}
