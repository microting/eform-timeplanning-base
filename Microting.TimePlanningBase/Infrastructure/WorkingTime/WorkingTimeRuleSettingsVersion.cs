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
using Microting.TimePlanningBase.Infrastructure.Data.Entities;

namespace Microting.TimePlanningBase.Infrastructure.WorkingTime;

public class WorkingTimeRuleSettingsVersion : PnBase
{
    public int WorkingTimeRuleSettingsId { get; set; }

    // Core working time
    public TimeSpan WeeklyNormalHours { get; set; } = TimeSpan.FromHours(37);

    // Optional: only used if OvertimeBasis uses Daily
    public TimeSpan DailyNormalHours { get; set; } = TimeSpan.FromMinutes(444); // 7h 24m

    // Rest rules
    public TimeSpan MinimumDailyRest { get; set; } = TimeSpan.FromHours(11);
    public TimeSpan MinimumWeeklyRest { get; set; } = TimeSpan.FromHours(24);
    public DayOfWeek WeekStartsOn { get; set; } = DayOfWeek.Monday;

    // Supplements
    public TimeSpan NightStart { get; set; } = new(17, 0, 0);
    public TimeSpan NightEnd { get; set; } = new(6, 0, 0);

    // Overtime calculation behavior
    public OvertimeBasis OvertimeBasis { get; set; } = OvertimeBasis.Weekly;

    // Break policy (optional but useful)
    public TimeSpan UnpaidBreakPerDay { get; set; } = TimeSpan.Zero;
    public TimeSpan PaidBreakPerDay { get; set; } = TimeSpan.Zero;

    // Meta / versioning (optional but recommended)
    public string RuleSetName { get; set; } = "Default";
    public int RuleSetVersion { get; set; } = 1;
}
