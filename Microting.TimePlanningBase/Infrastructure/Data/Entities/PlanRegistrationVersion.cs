﻿/*
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

namespace Microting.TimePlanningBase.Infrastructure.Data.Entities;

using System;

/// <summary>
/// PlanRegistrationVersion
/// </summary>
public class PlanRegistrationVersion : PnBase
{
    public int SdkSitId { get; set; }
    public DateTime Date { get; set; }
    public string PlanText { get; set; }
    public double PlanHours { get; set; }
    public int Start1Id { get; set; }
    public int Stop1Id { get; set; }
    public int Pause1Id { get; set; }
    public int Start2Id { get; set; }
    public int Stop2Id { get; set; }
    public int Pause2Id { get; set; }
    public int Start3Id { get; set; }
    public int Stop3Id { get; set; }
    public int Pause3Id { get; set; }
    public int Start4Id { get; set; }
    public int Stop4Id { get; set; }
    public int Pause4Id { get; set; }
    public int Start5Id { get; set; }
    public int Stop5Id { get; set; }
    public int Pause5Id { get; set; }
    public double NettoHours { get; set; }
    public double Flex { get; set; }
    public double SumFlexStart { get; set; }
    public double SumFlexEnd { get; set; }
    public double PaiedOutFlex { get; set; }
    public int? MessageId { get; set; }
    public string CommentOffice { get; set; }
    public string CommentOfficeAll { get; set; }
    public int PlanRegistrationId { get; set; }
    public int StatusCaseId { get; set; }
    public string WorkerComment { get; set; }
    public bool DataFromDevice { get; set; }
    public int? RegistrationDeviceId { get; set; }
    public DateTime? Start1StartedAt { get; set; }
    public DateTime? Stop1StoppedAt { get; set; }
    public DateTime? Pause1StartedAt { get; set; }
    public DateTime? Pause1StoppedAt { get; set; }
    public DateTime? Start2StartedAt { get; set; }
    public DateTime? Stop2StoppedAt { get; set; }
    public DateTime? Pause2StartedAt { get; set; }
    public DateTime? Pause2StoppedAt { get; set; }
    public DateTime? Pause10StartedAt { get; set; }
    public DateTime? Pause10StoppedAt { get; set; }
    public DateTime? Pause11StartedAt { get; set; }
    public DateTime? Pause11StoppedAt { get; set; }
    public DateTime? Pause12StartedAt { get; set; }
    public DateTime? Pause12StoppedAt { get; set; }
    public DateTime? Pause13StartedAt { get; set; }
    public DateTime? Pause13StoppedAt { get; set; }
    public DateTime? Pause14StartedAt { get; set; }
    public DateTime? Pause14StoppedAt { get; set; }
    public DateTime? Pause15StartedAt { get; set; }
    public DateTime? Pause15StoppedAt { get; set; }
    public DateTime? Pause16StartedAt { get; set; }
    public DateTime? Pause16StoppedAt { get; set; }
    public DateTime? Pause17StartedAt { get; set; }
    public DateTime? Pause17StoppedAt { get; set; }
    public DateTime? Pause18StartedAt { get; set; }
    public DateTime? Pause18StoppedAt { get; set; }
    public DateTime? Pause19StartedAt { get; set; }
    public DateTime? Pause19StoppedAt { get; set; }
    public DateTime? Pause100StartedAt { get; set; }
    public DateTime? Pause100StoppedAt { get; set; }
    public DateTime? Pause101StartedAt { get; set; }
    public DateTime? Pause101StoppedAt { get; set; }
    public DateTime? Pause102StartedAt { get; set; }
    public DateTime? Pause102StoppedAt { get; set; }
    public DateTime? Pause20StartedAt { get; set; }
    public DateTime? Pause20StoppedAt { get; set; }
    public DateTime? Pause21StartedAt { get; set; }
    public DateTime? Pause21StoppedAt { get; set; }
    public DateTime? Pause22StartedAt { get; set; }
    public DateTime? Pause22StoppedAt { get; set; }
    public DateTime? Pause23StartedAt { get; set; }
    public DateTime? Pause23StoppedAt { get; set; }
    public DateTime? Pause24StartedAt { get; set; }
    public DateTime? Pause24StoppedAt { get; set; }
    public DateTime? Pause25StartedAt { get; set; }
    public DateTime? Pause25StoppedAt { get; set; }
    public DateTime? Pause26StartedAt { get; set; }
    public DateTime? Pause26StoppedAt { get; set; }
    public DateTime? Pause27StartedAt { get; set; }
    public DateTime? Pause27StoppedAt { get; set; }
    public DateTime? Pause28StartedAt { get; set; }
    public DateTime? Pause28StoppedAt { get; set; }
    public DateTime? Pause29StartedAt { get; set; }
    public DateTime? Pause29StoppedAt { get; set; }
    public DateTime? Pause200StartedAt { get; set; }
    public DateTime? Pause200StoppedAt { get; set; }
    public DateTime? Pause201StartedAt { get; set; }
    public DateTime? Pause201StoppedAt { get; set; }
    public DateTime? Pause202StartedAt { get; set; }
    public DateTime? Pause202StoppedAt { get; set; }
    public int Shift1PauseNumber { get; set; }
    public int Shift2PauseNumber { get; set; }
    public int PlannedStartOfShift1 { get; set; }
    public int PlannedEndOfShift1 { get; set; }
    public int PlannedBreakOfShift1 { get; set; }
    public int PlannedStartOfShift2 { get; set; }
    public int PlannedEndOfShift2 { get; set; }
    public int PlannedBreakOfShift2 { get; set; }
    public bool IsDoubleShift { get; set; }
    public bool OnVacation { get; set; }
    public bool Sick { get; set; }
    public bool OtherAllowedAbsence { get; set; }
    public bool AbsenceWithoutPermission { get; set; }
    public DateTime? Start3StartedAt { get; set; }
    public DateTime? Stop3StoppedAt { get; set; }
    public DateTime? Pause3StartedAt { get; set; }
    public DateTime? Pause3StoppedAt { get; set; }
    public DateTime? Start4StartedAt { get; set; }
    public DateTime? Stop4StoppedAt { get; set; }
    public DateTime? Pause4StartedAt { get; set; }
    public DateTime? Pause4StoppedAt { get; set; }
    public DateTime? Start5StartedAt { get; set; }
    public DateTime? Stop5StoppedAt { get; set; }
    public DateTime? Pause5StartedAt { get; set; }
    public DateTime? Pause5StoppedAt { get; set; }
    public int PlannedStartOfShift3 { get; set; }
    public int PlannedEndOfShift3 { get; set; }
    public int PlannedBreakOfShift3 { get; set; }
    public int PlannedStartOfShift4 { get; set; }
    public int PlannedEndOfShift4 { get; set; }
    public int PlannedBreakOfShift4 { get; set; }
    public int PlannedStartOfShift5 { get; set; }
    public int PlannedEndOfShift5 { get; set; }
    public int PlannedBreakOfShift5 { get; set; }
    public bool PlanChangedByAdmin { get; set; }
    public int NettoHoursOverride { get; set; }
}