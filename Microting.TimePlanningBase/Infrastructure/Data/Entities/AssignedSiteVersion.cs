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

namespace Microting.TimePlanningBase.Infrastructure.Data.Entities;

public class AssignedSiteVersion: PnBase
{
    public int SiteId { get; set; }
    public int? CaseMicrotingUid { get; set; }
    public int AssignedSiteId { get; set; }

    public int? StartMonday { get; set; }
    public int? EndMonday { get; set; }
    public int? BreakMonday { get; set; }
    public int? StartTuesday { get; set; }
    public int? EndTuesday { get; set; }
    public int? BreakTuesday { get; set; }
    public int? StartWednesday { get; set; }
    public int? EndWednesday { get; set; }
    public int? BreakWednesday { get; set; }
    public int? StartThursday { get; set; }
    public int? EndThursday { get; set; }
    public int? BreakThursday { get; set; }
    public int? StartFriday { get; set; }
    public int? EndFriday { get; set; }
    public int? BreakFriday { get; set; }
    public int? StartSaturday { get; set; }
    public int? EndSaturday { get; set; }
    public int? BreakSaturday { get; set; }
    public int? StartSunday { get; set; }
    public int? EndSunday { get; set; }
    public int? BreakSunday { get; set; }

    public int MondayBreakMinutesDivider { get; set; }
    public int MondayBreakMinutesPrDivider { get; set; }
    public int TuesdayBreakMinutesDivider { get; set; }
    public int TuesdayBreakMinutesPrDivider { get; set; }
    public int WednesdayBreakMinutesDivider { get; set; }
    public int WednesdayBreakMinutesPrDivider { get; set; }
    public int ThursdayBreakMinutesDivider { get; set; }
    public int ThursdayBreakMinutesPrDivider { get; set; }
    public int FridayBreakMinutesDivider { get; set; }
    public int FridayBreakMinutesPrDivider { get; set; }
    public int SaturdayBreakMinutesDivider { get; set; }
    public int SaturdayBreakMinutesPrDivider { get; set; }
    public int SundayBreakMinutesDivider { get; set; }
    public int SundayBreakMinutesPrDivider { get; set; }
    public bool AutoBreakCalculationActive { get; set; }
    public int MondayBreakMinutesUpperLimit { get; set; }
    public int TuesdayBreakMinutesUpperLimit { get; set; }
    public int WednesdayBreakMinutesUpperLimit { get; set; }
    public int ThursdayBreakMinutesUpperLimit { get; set; }
    public int FridayBreakMinutesUpperLimit { get; set; }
    public int SaturdayBreakMinutesUpperLimit { get; set; }
    public int SundayBreakMinutesUpperLimit { get; set; }

    public int? StartMonday2NdShift { get; set; }
    public int? EndMonday2NdShift { get; set; }
    public int? BreakMonday2NdShift { get; set; }
    public int? StartTuesday2NdShift { get; set; }
    public int? EndTuesday2NdShift { get; set; }
    public int? BreakTuesday2NdShift { get; set; }
    public int? StartWednesday2NdShift { get; set; }
    public int? EndWednesday2NdShift { get; set; }
    public int? BreakWednesday2NdShift { get; set; }
    public int? StartThursday2NdShift { get; set; }
    public int? EndThursday2NdShift { get; set; }
    public int? BreakThursday2NdShift { get; set; }
    public int? StartFriday2NdShift { get; set; }
    public int? EndFriday2NdShift { get; set; }
    public int? BreakFriday2NdShift { get; set; }
    public int? StartSaturday2NdShift { get; set; }
    public int? EndSaturday2NdShift { get; set; }
    public int? BreakSaturday2NdShift { get; set; }
    public int? StartSunday2NdShift { get; set; }
    public int? EndSunday2NdShift { get; set; }
    public int? BreakSunday2NdShift { get; set; }
    public int? StartMonday3RdShift { get; set; }
    public int? EndMonday3RdShift { get; set; }
    public int? BreakMonday3RdShift { get; set; }
    public int? StartTuesday3RdShift { get; set; }
    public int? EndTuesday3RdShift { get; set; }
    public int? BreakTuesday3RdShift { get; set; }
    public int? StartWednesday3RdShift { get; set; }
    public int? EndWednesday3RdShift { get; set; }
    public int? BreakWednesday3RdShift { get; set; }
    public int? StartThursday3RdShift { get; set; }
    public int? EndThursday3RdShift { get; set; }
    public int? BreakThursday3RdShift { get; set; }
    public int? StartFriday3RdShift { get; set; }
    public int? EndFriday3RdShift { get; set; }
    public int? BreakFriday3RdShift { get; set; }
    public int? StartSaturday3RdShift { get; set; }
    public int? EndSaturday3RdShift { get; set; }
    public int? BreakSaturday3RdShift { get; set; }
    public int? StartSunday3RdShift { get; set; }
    public int? EndSunday3RdShift { get; set; }
    public int? BreakSunday3RdShift { get; set; }
    public int? StartMonday4ThShift { get; set; }
    public int? EndMonday4ThShift { get; set; }
    public int? BreakMonday4ThShift { get; set; }
    public int? StartTuesday4ThShift { get; set; }
    public int? EndTuesday4ThShift { get; set; }
    public int? BreakTuesday4ThShift { get; set; }
    public int? StartWednesday4ThShift { get; set; }
    public int? EndWednesday4ThShift { get; set; }
    public int? BreakWednesday4ThShift { get; set; }
    public int? StartThursday4ThShift { get; set; }
    public int? EndThursday4ThShift { get; set; }
    public int? BreakThursday4ThShift { get; set; }
    public int? StartFriday4ThShift { get; set; }
    public int? EndFriday4ThShift { get; set; }
    public int? BreakFriday4ThShift { get; set; }
    public int? StartSaturday4ThShift { get; set; }
    public int? EndSaturday4ThShift { get; set; }
    public int? BreakSaturday4ThShift { get; set; }
    public int? StartSunday4ThShift { get; set; }
    public int? EndSunday4ThShift { get; set; }
    public int? BreakSunday4ThShift { get; set; }
    public int? StartMonday5ThShift { get; set; }
    public int? EndMonday5ThShift { get; set; }
    public int? BreakMonday5ThShift { get; set; }
    public int? StartTuesday5ThShift { get; set; }
    public int? EndTuesday5ThShift { get; set; }
    public int? BreakTuesday5ThShift { get; set; }
    public int? StartWednesday5ThShift { get; set; }
    public int? EndWednesday5ThShift { get; set; }
    public int? BreakWednesday5ThShift { get; set; }
    public int? StartThursday5ThShift { get; set; }
    public int? EndThursday5ThShift { get; set; }
    public int? BreakThursday5ThShift { get; set; }
    public int? StartFriday5ThShift { get; set; }
    public int? EndFriday5ThShift { get; set; }
    public int? BreakFriday5ThShift { get; set; }
    public int? StartSaturday5ThShift { get; set; }
    public int? EndSaturday5ThShift { get; set; }
    public int? BreakSaturday5ThShift { get; set; }
    public int? StartSunday5ThShift { get; set; }
    public int? EndSunday5ThShift { get; set; }
    public int? BreakSunday5ThShift { get; set; }

    public int MondayPlanHours { get; set; }
    public int TuesdayPlanHours { get; set; }
    public int WednesdayPlanHours { get; set; }
    public int ThursdayPlanHours { get; set; }
    public int FridayPlanHours { get; set; }
    public int SaturdayPlanHours { get; set; }
    public int SundayPlanHours { get; set; }

    public bool Resigned { get; set; }
    public bool EnableMobileAccess { get; set; }
    public bool UseGoogleSheetAsDefault { get; set; } = true;
    public bool UseOnlyPlanHours { get; set; }
    public bool UsePunchClock { get; set; }
    public bool UseDetailedPauseEditing { get; set; }
    public bool UsePunchClockWithAllowRegisteringInHistory { get; set; }
    public bool ThirdShiftActive { get; set; }
    public bool FourthShiftActive { get; set; }
    public bool FifthShiftActive { get; set; }
    public bool DaysBackInTimeAllowedEditingEnabled { get; set; }
    public int DaysBackInTimeAllowedEditing { get; set; } = 2;
    public DateTime ResignedAtDate { get; set; }
    public bool UseOneMinuteIntervals { get; set; }
    public bool AllowAcceptOfPlannedHours { get; set; }
    public bool AllowEditOfRegistrations { get; set; }
    public bool AllowPersonalTimeRegistration { get; set; }
}