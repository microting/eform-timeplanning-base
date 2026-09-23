/*
The MIT License (MIT)

Copyright (c) 2007 - 2026 Microting A/S

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
using Microting.TimePlanningBase.Infrastructure.Data.Entities;

namespace Microting.TimePlanningBase.Infrastructure.Helpers;

/// <summary>
/// Maps <see cref="PlanTextParser"/> onto the flat shift columns of a
/// <see cref="PlanRegistration"/>.
///
/// This lives beside the parser rather than in each consumer because the
/// plugin and the background service both need it, and keeping one of them
/// out of step is how the parser it replaces came to have four divergent
/// copies.
/// </summary>
public static class PlanRegistrationPlanText
{
    private const int MinutesPerDay = 24 * 60;

    /// <summary>
    /// Parses <see cref="PlanRegistration.PlanText"/> into the shift columns
    /// and recomputes <see cref="PlanRegistration.PlanHours"/>.
    ///
    /// Every one of the five slots is written, so a segment removed from the
    /// text clears its shift instead of leaving a stale value behind. An empty
    /// or unparseable PlanText clears all five.
    /// </summary>
    public static void ParseInto(PlanRegistration planRegistration)
    {
        if (planRegistration is null)
        {
            return;
        }

        var slots = PlanTextParser.Parse(planRegistration.PlanText);

        for (var i = 0; i < PlanTextParser.MaxShifts; i++)
        {
            SetShift(planRegistration, i + 1, slots[i]);
        }

        // PlanText is the source of truth whenever it says anything, so the
        // recomputed total is written even when it is zero — otherwise text
        // that stops describing a shift clears the columns and leaves stale
        // hours behind, and nothing downstream notices the day changed.
        //
        // An empty PlanText is the one case that leaves PlanHours alone: the
        // sheet's separate hours column owns it then.
        if (!string.IsNullOrWhiteSpace(planRegistration.PlanText))
        {
            planRegistration.PlanHours = SumPlannedMinutes(planRegistration) / 60.0;
        }
    }

    /// <summary>
    /// Renders the shift columns back to a PlanText string. The inverse of
    /// <see cref="ParseInto"/> for every value the columns can hold.
    /// </summary>
    public static string Generate(PlanRegistration planRegistration) =>
        planRegistration is null ? string.Empty : PlanTextParser.Generate(ReadShifts(planRegistration));

    /// <summary>
    /// Recomputes <see cref="PlanRegistration.PlanHours"/> from the shift
    /// columns, deducting each shift's break.
    /// </summary>
    public static void RecalculatePlanHours(PlanRegistration planRegistration)
    {
        if (planRegistration is null)
        {
            return;
        }

        var totalMinutes = SumPlannedMinutes(planRegistration);

        // Callers that reach this directly, rather than through ParseInto, keep
        // the long-standing behaviour of leaving PlanHours alone when the
        // columns describe nothing.
        if (totalMinutes > 0)
        {
            planRegistration.PlanHours = totalMinutes / 60.0;
        }
    }

    /// <summary>
    /// Total planned minutes across the five shifts, each less its break.
    /// </summary>
    private static int SumPlannedMinutes(PlanRegistration planRegistration)
    {
        var totalMinutes = 0;

        foreach (var slot in ReadShifts(planRegistration))
        {
            if (slot is not { } shift)
            {
                continue;
            }

            var start = shift.StartMinutes;
            var end = shift.EndMinutes;

            // An empty row, and a shift that begins and ends at the same time,
            // both describe no work.
            if (start == end)
            {
                continue;
            }

            // An end before the start crosses midnight: "22:00-0:00" is two
            // hours, not minus twenty-two. Without this the day's total goes
            // negative and drags any other shift down with it.
            if (end < start)
            {
                end += MinutesPerDay;
            }

            totalMinutes += end - start - shift.BreakMinutes;
        }

        return totalMinutes < 0 ? 0 : totalMinutes;
    }

    private static IEnumerable<PlanTextParser.Shift?> ReadShifts(PlanRegistration reg)
    {
        for (var i = 1; i <= PlanTextParser.MaxShifts; i++)
        {
            yield return GetShift(reg, i);
        }
    }

    private static PlanTextParser.Shift? GetShift(PlanRegistration reg, int shiftNumber) => shiftNumber switch
    {
        1 => new PlanTextParser.Shift(reg.PlannedStartOfShift1, reg.PlannedEndOfShift1, reg.PlannedBreakOfShift1),
        2 => new PlanTextParser.Shift(reg.PlannedStartOfShift2, reg.PlannedEndOfShift2, reg.PlannedBreakOfShift2),
        3 => new PlanTextParser.Shift(reg.PlannedStartOfShift3, reg.PlannedEndOfShift3, reg.PlannedBreakOfShift3),
        4 => new PlanTextParser.Shift(reg.PlannedStartOfShift4, reg.PlannedEndOfShift4, reg.PlannedBreakOfShift4),
        5 => new PlanTextParser.Shift(reg.PlannedStartOfShift5, reg.PlannedEndOfShift5, reg.PlannedBreakOfShift5),
        _ => null
    };

    private static void SetShift(PlanRegistration reg, int shiftNumber, PlanTextParser.Shift? slot)
    {
        var shift = slot ?? new PlanTextParser.Shift(0, 0, 0);

        switch (shiftNumber)
        {
            case 1:
                reg.PlannedStartOfShift1 = shift.StartMinutes;
                reg.PlannedEndOfShift1 = shift.EndMinutes;
                reg.PlannedBreakOfShift1 = shift.BreakMinutes;
                break;
            case 2:
                reg.PlannedStartOfShift2 = shift.StartMinutes;
                reg.PlannedEndOfShift2 = shift.EndMinutes;
                reg.PlannedBreakOfShift2 = shift.BreakMinutes;
                break;
            case 3:
                reg.PlannedStartOfShift3 = shift.StartMinutes;
                reg.PlannedEndOfShift3 = shift.EndMinutes;
                reg.PlannedBreakOfShift3 = shift.BreakMinutes;
                break;
            case 4:
                reg.PlannedStartOfShift4 = shift.StartMinutes;
                reg.PlannedEndOfShift4 = shift.EndMinutes;
                reg.PlannedBreakOfShift4 = shift.BreakMinutes;
                break;
            case 5:
                reg.PlannedStartOfShift5 = shift.StartMinutes;
                reg.PlannedEndOfShift5 = shift.EndMinutes;
                reg.PlannedBreakOfShift5 = shift.BreakMinutes;
                break;
        }
    }
}
